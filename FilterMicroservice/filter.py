import paho.mqtt.client as paho
import json
import pandas as pd
from datetime import datetime, timedelta
import threading
import asyncio
import nats

sensor_topic = "sensor/pond"
aggregation_window = 180  # seconds
global_df = pd.DataFrame(
    columns=[
        "PondId",
        "CreatedAt",
        "EntryId",
        "Temperature_C",
        "Turbidity_ntu",
        "DissolvedOxygen_g_mL",
        "pH",
        "Ammonia_g_mL",
        "Nitrite_g_mL",
        "Population",
        "FishLength_cm",
        "FishWeight_g",
    ]
)

lock = threading.Lock()


async def publish_aggregated_data(aggregated_data):
    nc = await nats.connect("nats://localhost:4222")
    aggregated_data["StartTime"] = aggregated_data["StartTime"].strftime(
        "%Y-%m-%d %H:%M:%S"
    )
    aggregated_data["EndTime"] = aggregated_data["EndTime"].strftime(
        "%Y-%m-%d %H:%M:%S"
    )
    await nc.publish("aggregated_data", json.dumps(aggregated_data).encode())
    await nc.flush()
    await nc.close()


def on_connect(client, userdata, flags, rc):
    print("Connected with result code " + str(rc))
    client.subscribe(sensor_topic + "/+")


def on_message(client, userdata, msg):
    if msg.topic.startswith(sensor_topic):
        dict_data = json.loads(msg.payload)
        dict_data["CreatedAt"] = pd.to_datetime(dict_data["CreatedAt"], utc=True)
        with lock:
            global global_df
            global_df = pd.concat(
                [global_df, pd.DataFrame([dict_data])], ignore_index=True
            )

        # before aggregation we need to check whether the data in the window enough for aggregation
        # aggregation is performed every 3 minutes
        # after aggregation, data is sent to the next microservice
        # dataframe is reset to empty

        if global_df["CreatedAt"].max() - global_df["CreatedAt"].min() > timedelta(
            seconds=aggregation_window
        ):
            print(global_df["CreatedAt"].max(), global_df["CreatedAt"].min())
            aggregate_data(global_df)

            with lock:
                global_df.drop(global_df.index, inplace=True)
    else:
        print("Message received from an unknown topic:", msg.topic)


def on_publish(client, userdata, mid):
    print("mid: " + str(mid))


def aggregate_data(df):
    if df.empty:
        return None

    df.set_index("CreatedAt", inplace=True)

    aggregated_df = df.apply(
        {
            "Temperature_C": "mean",
            "Turbidity_ntu": "mean",
            "DissolvedOxygen_g_mL": "mean",
            "pH": "mean",
            "Ammonia_g_mL": "mean",
            "Nitrite_g_mL": "mean",
            "Population": "max",
            "FishLength_cm": "mean",
            "FishWeight_g": "mean",
        }
    )

    aggregated_df["PondId"] = df["PondId"].iloc[0]
    aggregated_df["PondId"] = aggregated_df["PondId"].astype(int)

    aggregated_df["StartTime"] = df.index.min()
    aggregated_df["EndTime"] = df.index.max()

    df.reset_index(inplace=True)

    print("Aggregated Data:")
    print(aggregated_df)

    asyncio.run(publish_aggregated_data(aggregated_df.to_dict()))


def start_filter():
    client = paho.Client()
    client.on_connect = on_connect
    client.on_message = on_message
    client.on_publish = on_publish

    client.connect("localhost", 8883, 60)

    client.loop_forever()


if __name__ == "__main__":
    start_filter()

import argparse
import time
import threading
import os
import paho.mqtt.publish as publish
import json

sensor_topic = "sensor/pond"


def get_files():
    directory = os.path.dirname(os.path.realpath(__file__))
    ponds = []
    path = os.path.join(directory, "dataset")
    for file in os.listdir(path):
        if file.endswith(".csv"):
            ponds.append(os.path.join(path, file))
    return ponds


def load_data_from_csv(pond_path, limit, offset):
    with open(pond_path, "r") as file:
        lines = file.readlines()
        lines = lines[1:]
        lines = lines[offset : offset + limit]

        for line in lines:
            print(threading.current_thread().name, line)
            line_values = line.split(",")

            data = {
                "PondId": int(line_values[0]),
                "CreatedAt": line_values[1],
                "EntryId": int(line_values[2]),
                "Temperature_C": float(line_values[3]),
                "Turbidity_ntu": int(line_values[4]),
                "DissolvedOxygen_g_mL": float(line_values[5]),
                "pH": float(line_values[6]),
                "Ammonia_g_mL": float(line_values[7]),
                "Nitrite_g_mL": float(line_values[8]),
                "Population": int(line_values[9]),
                "FishLength_cm": float(line_values[10]),
                "FishWeight_g": float(line_values[11]),
            }
            json_data = json.dumps(data)

            try:
                publish.single(
                    sensor_topic + "/" + str(data["PondId"]),
                    payload=json_data,
                    hostname="mosquitto",
                    port=8883,
                    client_id="",
                    keepalive=60,
                    qos=0,
                    retain=False,
                )
            except Exception as e:
                print("Error while publishing message", e)


def process_pond_data(pond_path, limit, sleep):
    offset = 0

    while True:
        load_data_from_csv(pond_path, limit, offset)
        offset += limit
        time.sleep(sleep)


def start_server(ponds, limit, sleep):
    threads = []

    for pond in ponds:
        thread = threading.Thread(target=process_pond_data, args=(pond, limit, sleep))
        threads.append(thread)
        thread.start()

    for thread in threads:
        thread.join()


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--limit", type=int, default=5, help="Limit the number of records to fetch"
    )
    parser.add_argument(
        "--sleep", type=int, default=10, help="Sleep time in seconds between each fetch"
    )

    args = parser.parse_args()

    limit = args.limit
    sleep = args.sleep

    ponds = get_files()
    time.sleep(5)

    start_server(ponds, limit, sleep)

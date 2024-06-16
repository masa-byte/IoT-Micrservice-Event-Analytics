# Microservice Application for IOT 
This is part three of a project for course **Internet of Things and Services**
<br>
<br>

### Application type
This application has its own web interface for checking the results, and visualizations can be done with **Grafana**. It is a **microservice** application, meaning it is divided into multiple services, each with its own responsibility.
<br>

### Idea
The idea is to have 3 microservices that communicate with each other using MQTT, NATS, and one of them has web interface for the clients to see the events in real time.

### Architecture
The first microservice is written in **Python** and it is the Server microservice. It simulates data stream from a csv file and sends that to an **MQTT** topic. The second microservice is written in **Python** and it is the Filter microservice. It reads the data from the MQTT topic and filters it based on the time window, sending the aggregated data to a NATS topic. The third microservice is written in **.NET Core** and it is the Command microservice. It is subscribed to the **MQTT** topic on which **eKuiper** send analysis data, and has a web interface to display the events to the clients. it uses **SignalR** to do that. The fourth microservice is written in **NestJS** and it is subscribed to the NATS topic. It collects the data, stores in it **InfluxDB**. Lastly, **Grafana** is used for visualization.

### Data
The database is **InfluxDB**. The dataset used is https://www.kaggle.com/datasets/ogbuokiriblessing/sensor-based-aquaponics-fish-pond-datasets?select=IoTPond10.csv. The dataset is about the sensor data from aquaponics fish ponds. The data is about the temperature, pH, oxygen level, etc. in the water. I have cleaned the data and added pond_id to the data. The data is stored in csv files and the Server Microservice reads from them and simulates real time stream.

### Tech stack
It was written in **.NET Core**, **NestJS**, and **Python**, and uses **InfluxDB** as the database, and **MQTT** and **NATS**. MQTT broker is **eclipse-mosquitto**. **eKuiper** is used for analytics. **Docker** is used for containerization. **Docker-compose** is used for orchestration. **Grafana** is used for visualization. **SignalR** is used for real time communication.
<br>
<br>

### How to run the application
1. Clone the repository
2. Open the repository in Visual Studio Code
3. Open the terminal in Visual Studio Code
4. Run the following command:
    - `docker-compose up`
5. Wait for the services to start
6. The application is available on http://localhost:5000/
7. InfluxDB GUI is available on http://localhost:8086/ 
    - Username: masa
    - Password: masaadmin
8. EKuiper GUI is available on http://localhost:9082/ 
    - Username: admin
    - Password: public
12. Grafana GUI is available on http://localhost:3000/
    - Username: admin
    - Password: admin
Query for Grafana:
```
from(bucket: "bucket")
  |> range(start: v.timeRangeStart, stop: v.timeRangeStop)
  |> filter(fn: (r) => r["_measurement"] == "agg_pond_data")
  |> filter(fn: (r) => r["PondId"] == "1")
  |> filter(fn: (r) => r["_field"] == "Ammonia_g_mL" or r["_field"] == "DissolvedOxygen_g_mL" or r["_field"] == "Duration" or r["_field"] == "Nitrite_g_mL" or r["_field"] == "Population" or r["_field"] == "FishLength_cm" or r["_field"] == "FishWeight_g" or r["_field"] == "Temperature_C" or r["_field"] == "Turbidity_ntu" or r["_field"] == "pH")
```
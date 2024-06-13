# Microservice Application for IOT 
This is part two of a project for course **Internet of Things and Services**
<br>
<br>

### Application type
This application is tested through clients like **Postman**, and **Insomnia** and **Swagger**. It is a **microservice** application, meaning it is divided into multiple services, each with its own responsibility.
<br>

### Idea
The idea is to have 3 microservices that communicate with each other using MQTT, and one of them has REST API for communication with clients.

### Architecture
The first microservice was written in **Python** and it is acting as a sensor, which is reading the data from a database (simulating the way data would arrive in real-time) and sending that to a MQTT topic. The second microservice was written in **Python** as well and it is acting as an analytics service, which is collecting the data from sensor and is sending it to **eKuiper** for analysis, and then forwarding results to another MQTT topic. The last microservice is acting as an access point for clients, meaning it implements REST API and is subscribing to the topic with event info from the analytics service. The last microservice is written in **.NET Core**.

### Data
The database is **PostgreSQL**. The dataset used is https://www.kaggle.com/datasets/ogbuokiriblessing/sensor-based-aquaponics-fish-pond-datasets?select=IoTPond10.csv. The dataset is about the sensor data from aquaponics fish ponds. The data is about the temperature, pH, oxygen level, etc. in the water. I have cleaned the data and added pond_id to the data, and when importing the data, entry_id will be overridden to keep consistency and unique keys. The data is stored in the database, and the sensor microservice reads from it.

### Tech stack
It was written in **.NET Core** and **Python**, and uses **PostgreSQL** as the database, and **MQTT** and **REST**. MQTT broker is **eclipse-mosquitto**. **eKuiper** is used for analytics. **Docker** is used for containerization. **Docker-compose** is used for orchestration. **pgAdmin** is used for database GUI. **Swagger** is used for API documentation. **Postman** and **Insomnia** are used for testing.
<br>
<br>

### How to run the application
1. Clone the repository
2. Open the repository in Visual Studio Code
3. Open the terminal in Visual Studio Code
4. Inside of EventInfo, run the following command:
    - `dotnet ef database update`
5. Run the following command:
    - `docker-compose up`
6. Wait for the services to start
7. Run the import-data.py script to import the data into the database
8. Sensor service needs to be restarted to start sending data to MQTT
9. To test the REST API, use Postman or Insomnia
    - GET http://localhost:5117/messages
    - GET http://localhost:5117/messages/{id}
    - GET http://localhost:5117/messages/phAlerts
    - GET http://localhost:5117/messages/temperatureAlerts
10. Database GUI is available on http://localhost:5050/ 
    - Username: admin@gmail.com
    - Password: 123
11. EKuiper GUI is available on http://localhost:9082/ 
    - Username: admin
    - Password: public

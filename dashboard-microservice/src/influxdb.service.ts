import { InfluxDB, Point, WriteApi } from '@influxdata/influxdb-client';
import { Injectable, OnModuleInit } from '@nestjs/common';
import { INFLUXDB_URL, INFLUXDB_TOKEN, INFLUXDB_ORG, INFLUXDB_BUCKET } from './config';

@Injectable()
export class InfluxDBService implements OnModuleInit {
  private influxDB: InfluxDB;
  private writeApi: WriteApi;

  constructor() {
    const url = INFLUXDB_URL
    const token = INFLUXDB_TOKEN
    const org = INFLUXDB_ORG
    const bucket = INFLUXDB_BUCKET

    this.influxDB = new InfluxDB({ url, token });
    this.writeApi = this.influxDB.getWriteApi(org, bucket);
  }

  onModuleInit() {
  }

  async writeAggregatedData(data: any) {
    const startTime = new Date(data.StartTime);
    const endTime = new Date(data.EndTime);
    const duration = (endTime.getTime() - startTime.getTime()) / 1000;

    const point = new Point('agg_pond_data')
      .tag('PondId', data.PondId.toString())
      .floatField('Temperature_C', data.Temperature_C)
      .floatField('Turbidity_ntu', data.Turbidity_ntu)
      .floatField('DissolvedOxygen_g_mL', data.DissolvedOxygen_g_mL)
      .floatField('pH', data.pH)
      .floatField('Ammonia_g_mL', data.Ammonia_g_mL)
      .floatField('Nitrite_g_mL', data.Nitrite_g_mL)
      .intField('Population', data.Population)
      .floatField('FishLength_cm', data.FishLength_cm)
      .floatField('FishWeight_g', data.FishWeight_g)
      .intField('Duration', duration)
      .timestamp(new Date(data.StartTime));

    try {
      this.writeApi.writePoint(point);
      await this.writeApi.flush();
      console.log('Data written successfully to InfluxDB.');
    } catch (error) {
      console.error('Error writing to InfluxDB:', error);
    }
  }
}

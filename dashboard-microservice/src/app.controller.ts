import { Controller, Get } from '@nestjs/common';
import { InfluxDBService } from './influxdb.service';
import { EventPattern } from '@nestjs/microservices';

@Controller()
export class AppController {
  constructor(private readonly influxDBService: InfluxDBService) { }

  @EventPattern('aggregated_data')
  async handleAggregatedData(data: Record<string, unknown>) {
    console.log('Aggregated data received:', data);
    await this.influxDBService.writeAggregatedData(data);
  }
}

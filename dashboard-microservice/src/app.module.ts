import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { InfluxDBService } from './influxdb.service';

@Module({
  imports: [],
  controllers: [AppController],
  providers: [InfluxDBService],
})
export class AppModule { }

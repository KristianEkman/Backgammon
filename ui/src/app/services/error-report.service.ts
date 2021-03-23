import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ErrorReportDto } from '../dto';

@Injectable({
  providedIn: 'root'
})
export class ErrorReportService {
  url: string;
  constructor(private httpClient: HttpClient) {
    this.url = `${environment.apiServiceUrl}/errorreport`;
  }

  saveErrorReport(dto: ErrorReportDto): void {
    this.httpClient.post<ErrorReportDto>(this.url, dto).subscribe();
  }
}

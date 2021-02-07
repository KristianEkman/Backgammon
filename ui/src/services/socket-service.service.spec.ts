import { TestBed } from '@angular/core/testing';

import { SocketServiceService } from './socket-service.service';

describe('SocketServiceService', () => {
  let service: SocketServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SocketServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

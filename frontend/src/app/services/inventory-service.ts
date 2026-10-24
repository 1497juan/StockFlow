
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Product {
  id: number;
  sku: string;
  name: string;
  description: string | null;
  unitPrice: number;
  stock: number;
}

export interface InventoryMovement {
  productId: number;
  quantity: number;
}

@Injectable({
  providedIn: 'root',
})
export class InventoryService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5054/api/Inventory';

  getCurrentInventory(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  registerEntry( movement: InventoryMovement): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/entries`, movement);
  }

  registerExit( movement: InventoryMovement): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/exits`, movement);
  }
}

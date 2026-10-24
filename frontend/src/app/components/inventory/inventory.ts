import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { InventoryService, Product } from '../../services/inventory-service';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth-service';

type MovementType = 'entry' | 'exit';

@Component({
  imports: [ CommonModule, ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, MatRadioModule, MatSelectModule],
  selector: 'app-inventory',
  styleUrl: './inventory.scss',
  templateUrl: './inventory.html',
})
export class Inventory implements OnInit {

  products: Product[] = [];
  loadingProducts = false;
  submitting = false;
  errorMessage = '';
  successMessage = '';
  
  readonly movementForm;

    constructor( private readonly fb: FormBuilder, 
      private readonly inventoryService: InventoryService, 
      private readonly cdr: ChangeDetectorRef,
      private readonly authService: AuthService,
      private readonly router: Router)
    {
        this.loadInventory();

    this.movementForm = this.fb.group({
      type: this.fb.nonNullable.control<MovementType>('entry'),

      productId: this.fb.control<number | null>(
        null,
        Validators.required
      ),

      quantity: this.fb.nonNullable.control(10, [
        Validators.required,
        Validators.min(1),
        Validators.pattern(/^[1-9]\d*$/)
      ])
    });
    }
  

  ngOnInit(): void {
    this.loadInventory();
  }

loadInventory(): void {
  this.loadingProducts = true;
  this.errorMessage = '';

  this.inventoryService.getCurrentInventory().subscribe({
    next: (products) => {
      console.log(products);

      this.products = [...products];
      this.loadingProducts = false;

      this.cdr.markForCheck();
    },
    error: (error) => {
      console.error('Error cargando productos:', error);

      this.errorMessage =
        'No fue posible cargar el inventario. Verifica la conexión con el servidor.';

      this.loadingProducts = false;

      this.cdr.markForCheck();
    },
  });
}

  onSubmit(): void {
    if (this.movementForm.invalid || this.submitting) {
      this.movementForm.markAllAsTouched();
      return;
    }

    const { type, productId, quantity } =
      this.movementForm.getRawValue();

    if (productId === null || quantity < 1) {
      return;
    }

    const movement = { productId, quantity };

    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const request =
      type === 'entry'
        ? this.inventoryService.registerEntry(movement)
        : this.inventoryService.registerExit(movement);

    request.subscribe({
      next: () => {
        this.successMessage =
          type === 'entry'
            ? 'Entrada de inventario registrada correctamente.'
            : 'Salida de inventario registrada correctamente.';

        this.submitting = false;
        this.movementForm.controls.quantity.setValue(1);
        this.loadInventory();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message ??
          error.error?.detail ??
          'No fue posible registrar el movimiento. Verifica el stock y vuelve a intentarlo.';

        this.submitting = false;
      },
    });
  }

  logout(): void {
  this.authService.logout();
  this.router.navigate(['/login']);
  }

  
}

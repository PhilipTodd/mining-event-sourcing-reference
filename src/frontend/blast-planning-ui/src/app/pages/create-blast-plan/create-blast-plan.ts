import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { NgIf } from '@angular/common';

import { BlastPlanApiService } from '../../core/services/blast-plan-api.service';

@Component({
  selector: 'app-create-blast-plan',
  imports: [
    NgIf,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './create-blast-plan.html',
  styleUrl: './create-blast-plan.scss',
})
export class CreateBlastPlan {
  readonly form;

  isSubmitting = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly api: BlastPlanApiService,
    private readonly router: Router,
    private readonly snackBar: MatSnackBar) {
    this.form = this.fb.nonNullable.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      siteId: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    this.api.createBlastPlan(this.form.getRawValue()).subscribe({
      next: result => {
        this.snackBar.open('Blast plan created', 'Dismiss', { duration: 3000 });
        this.router.navigate(['/blast-plans', result.blastPlanId]);
      },
      error: () => {
        this.isSubmitting = false;
        this.snackBar.open('Failed to create blast plan', 'Dismiss', { duration: 5000 });
      }
    });
  }
}

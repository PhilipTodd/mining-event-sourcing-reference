import { NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-blast-plan-lookup',
  imports: [
    NgIf,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './blast-plan-lookup.html',
  styleUrl: './blast-plan-lookup.scss'
})
export class BlastPlanLookup {
  readonly form;

  constructor(
    private readonly fb: FormBuilder,
    private readonly router: Router) {
    this.form = this.fb.nonNullable.group({
      blastPlanId: ['', [Validators.required]]
    });
  }

  search(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.router.navigate([
      '/blast-plans',
      this.form.getRawValue().blastPlanId
    ]);
  }
}

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {providePrimeNG} from "primeng/config";
import Material from "@primeuix/themes/material";
import {Button} from "primeng/button";
import {QuizPageComponent} from "./features/components/quiz-page/quiz-page.component";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    Button,
    QuizPageComponent
  ],
  providers: [
    providePrimeNG({
      theme: {
        preset: Material,
      },
    }),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

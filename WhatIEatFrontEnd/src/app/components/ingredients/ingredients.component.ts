import { Component, OnInit } from '@angular/core';
import { DataService } from '../../services/data.service';

@Component({
  selector: 'app-ingredients',
  templateUrl: './ingredients.component.html',
  styleUrls: ['./ingredients.component.css']
})
export class IngredientsComponent implements OnInit {
  
  //public profileObject1: any = {IngredientName: 'test', IngredientDescription: '', 
  //                              IngredientDangerLevel: 0 , FuzzyDistance: 0};

  profileObjects: ing[] = []; // Create and initialize an array of objects
  //profileObject:Object[]=[]; 

  constructor(private dataService:DataService) { }

  ngOnInit() {

    console.log('ngOnInit from Ingredients conponent starts...');
    this.dataService.postIngredients()
                    .subscribe(res => this.profileObjects = res);
    
  }

}

interface ing{
  IngredientName: string;
  IngredientDescription: string;
  IngredientDangerLevel: number;
  FuzzyDistance: number;
  }
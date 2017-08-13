import { Component, OnInit } from '@angular/core';
import { DataService } from '../../services/data.service';

@Component({
  selector: 'app-ingredients',
  templateUrl: './ingredients.component.html',
  styleUrls: ['./ingredients.component.css']
})
export class IngredientsComponent implements OnInit {
  
  public profileObject: any = {IngredientName: 'test', IngredientDescription: '', 
                                IngredientDangerLevel: 0 , FuzzyDistance: 0};

  constructor(private dataService:DataService) { }

  ngOnInit() {
    console.log('ngOnInit from Ingredients conponent starts...');
    this.dataService.postIngredients().subscribe(res => this.profileObject = res);
  }

}

import {Component, View} from 'angular2/angular2';
import {Task, TaskEventsBuilder} from './task.ts';
var taskEvents = new TaskEventsBuilder().build();
@Component({
  selector:'todo-input'
})
@View({
  template:`
    <input type="text" #log-me/>
    <button (click)="onClick(logMe)" >Log Input</button>
  `
})
export class TodoInput{
  constructor(){
    taskEvents.onTaskRemoved(t => console.log("the task '"+ t.title+ "' has been removed."))
  }
  onClick(logMe){
    taskEvents.taskAdded(new Task(logMe.value));
  }
}

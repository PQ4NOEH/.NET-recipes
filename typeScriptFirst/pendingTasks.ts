import {Component, View,CORE_DIRECTIVES} from "angular2/angular2";
import {Task, TaskEventsBuilder} from './task.ts';
var taskEvents = new TaskEventsBuilder().build();
@Component({
  selector:'pending-tasks'
})
@View({
  directives: [CORE_DIRECTIVES],
  template:`
    <div>
    pending tasks
      <div *ng-for="#todo of items">
          {{todo.title}}<button (click)="removeTask(todo)">*</button>
      </div>
    </div>
  `
})
export class PendingTasks{
  items:Task[];
  constructor(){
    taskEvents.onTaskAdded(t=> this.items.push(t))
     this.items =[];
   }
   removeTask(task:Task){
     var index = this.items.indexOf(task);
     this.items.splice(index,1);
     taskEvents.taskRemoved(task);
   }
};

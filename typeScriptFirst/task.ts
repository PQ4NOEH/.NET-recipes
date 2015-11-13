
///<reference path="node_modules/rx/ts/rx.d.ts" />
// var onTaskAddedSubscriptions = [];
// var onTaskRemovedSubscriptions = [];
//import {Rx} from 'rx.d.ts';
var taskAddedObserver;
var taskRemovedObserver;
class TaskEvents{
  taskAddedSequence
  taskRemovedSequence
  constructor(){
    this.taskAddedSequence = Rx.Observable.create(function(observer){ taskAddedObserver = observer;});
    this.taskRemovedSequence = Rx.Observable.create(function(observer){ taskRemovedObserver = observer;});
  }
  taskAdded(task:Task){
    //onTaskAddedSubscriptions.forEach((cb)=> cb(task));
    taskAddedObserver.onNext(Task);
  }
  taskRemoved(task:Task){
    taskRemovedObserver.onNext(Task);
    // onTaskRemovedSubscriptions.forEach((cb)=> cb(task));
  }
  // onTaskAdded(cb:(t:Task)=> any){
  //   onTaskAddedSubscriptions.push(cb);
  // }
  // onTaskRemoved(cb:(t:Task)=> any){
  //   onTaskRemovedSubscriptions.push(cb);
  // }
}

var taskEventsInstance =  new TaskEvents();
export class TaskEventsBuilder{
  build():TaskEvents{
    return taskEventsInstance;
  }
}

export class Task{
  title:string;
  constructor(title:string){
    this.title = title;
  }
}

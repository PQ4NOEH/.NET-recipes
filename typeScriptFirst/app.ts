import {bootstrap, Component, View} from "angular2/angular2";
import {TodoInput} from "./todoInput.ts";
import {PendingTasks} from "./pendingTasks.ts";
@Component({
  selector:'app'
})
@View({
  directives:[TodoInput, PendingTasks],
  template:`
  <todo-input></todo-input>
  <pending-tasks></pending-tasks>
  `
})
class App{}

bootstrap(App);

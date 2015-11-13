var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var angular2_1 = require('angular2/angular2');
var task_ts_1 = require('./task.ts');
var taskEvents = new task_ts_1.TaskEventsBuilder().build();
var TodoInput = (function () {
    function TodoInput() {
        taskEvents.onTaskRemoved(function (t) { return console.log("the task '" + t.title + "' has been removed."); });
    }
    TodoInput.prototype.onClick = function (logMe) {
        taskEvents.taskAdded(new task_ts_1.Task(logMe.value));
    };
    TodoInput = __decorate([
        angular2_1.Component({
            selector: 'todo-input'
        }),
        angular2_1.View({
            template: "\n    <input type=\"text\" #log-me/>\n    <button (click)=\"onClick(logMe)\" >Log Input</button>\n  "
        }), 
        __metadata('design:paramtypes', [])
    ], TodoInput);
    return TodoInput;
})();
exports.TodoInput = TodoInput;

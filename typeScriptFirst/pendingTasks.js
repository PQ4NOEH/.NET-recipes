var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var angular2_1 = require("angular2/angular2");
var task_ts_1 = require('./task.ts');
var taskEvents = new task_ts_1.TaskEventsBuilder().build();
var PendingTasks = (function () {
    function PendingTasks() {
        var _this = this;
        taskEvents.onTaskAdded(function (t) { return _this.items.push(t); });
        this.items = [];
    }
    PendingTasks.prototype.removeTask = function (task) {
        var index = this.items.indexOf(task);
        this.items.splice(index, 1);
        taskEvents.taskRemoved(task);
    };
    PendingTasks = __decorate([
        angular2_1.Component({
            selector: 'pending-tasks'
        }),
        angular2_1.View({
            directives: [angular2_1.CORE_DIRECTIVES],
            template: "\n    <div>\n    pending tasks\n      <div *ng-for=\"#todo of items\">\n          {{todo.title}}<button (click)=\"removeTask(todo)\">*</button>\n      </div>\n    </div>\n  "
        }), 
        __metadata('design:paramtypes', [])
    ], PendingTasks);
    return PendingTasks;
})();
exports.PendingTasks = PendingTasks;
;

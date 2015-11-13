var taskAddedObserver;
var taskRemovedObserver;
var TaskEvents = (function () {
    function TaskEvents() {
        this.taskAddedSequence = Rx.Observable.create(function (observer) { taskAddedObserver = observer; });
        this.taskRemovedSequence = Rx.Observable.create(function (observer) { taskRemovedObserver = observer; });
    }
    TaskEvents.prototype.taskAdded = function (task) {
        taskAddedObserver.onNext(Task);
    };
    TaskEvents.prototype.taskRemoved = function (task) {
        taskRemovedObserver.onNext(Task);
    };
    return TaskEvents;
})();
var taskEventsInstance = new TaskEvents();
var TaskEventsBuilder = (function () {
    function TaskEventsBuilder() {
    }
    TaskEventsBuilder.prototype.build = function () {
        return taskEventsInstance;
    };
    return TaskEventsBuilder;
})();
exports.TaskEventsBuilder = TaskEventsBuilder;
var Task = (function () {
    function Task(title) {
        this.title = title;
    }
    return Task;
})();
exports.Task = Task;

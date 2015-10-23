var Bus;
(function (Bus) {
    var CommandBus = (function () {
        function CommandBus() {
        }
        CommandBus.prototype.ExecuteCommand = function (command) {
        };
        return CommandBus;
    })();
    Bus.CommandBus = CommandBus;
})(Bus || (Bus = {}));
function g(limit) {
    for (var i = 0; i < limit; i++) {
        yield i;
    }
}

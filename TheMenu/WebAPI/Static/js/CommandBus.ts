module Bus {
    export class CommandBus {
        ExecuteCommand(command: ICommand) {
        }
    }
    export interface ICommand {
        url: string;
        commandName: string;
    }
}


function *g(limit: number) : Iterable<number> {
    for (var i = 0; i < limit; i++) {
        yield i;
    }
}
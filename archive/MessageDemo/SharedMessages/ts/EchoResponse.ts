///<reference path='../../../Libs/MessageLib/ts/Messages.ts'/>

// use if using "export module"
// import E = require('../../../Libs/MessageLib/ts/Messages');

module SharedMessages {

    export class EchoResponse extends MessageLib.Message {
        Text: string;
    }

}
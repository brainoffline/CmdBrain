namespace No8.CmdBrain;

public interface Model
{
    Cmd? Init();
    (Model, Cmd?) Update(Msg msg);
    string View();
}

public interface Model<T> : Model { }



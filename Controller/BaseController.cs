namespace ServerFramework.Controller
{
    class BaseController : IController
    {
        protected const char Separator = ',';
        protected const char VerticalBar = '|';
        protected const string EmptyStr = "";

        public virtual void Update() { }
    }
}

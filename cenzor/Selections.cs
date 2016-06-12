namespace cenzor
{
    public class Selections
    {

        public string temporaryFilesPath
        {
            get; private set;
        }
        public int beginX
        {
            get; private set;
        }
        public int endX
        {
            get; private set;
        }
        public int beginY
        {
            get; private set;
        }
        public int endY
        {
            get; private set;
        }
        public int BeginSelection
        {
            get; private set;
        }
        public int EndSelection
        {
            get; private set;
        }

        public Selections(
            int beginX, 
            int endX, 
            int beginY, 
            int endY, 
            int beginSelection, 
            int endSelection, 
            string temporaryFilesPath)
        {
            this.beginX = beginX;
            this.endX = endX;
            this.beginY = beginY;
            this.endY = endY;
            this.BeginSelection = beginSelection;
            this.EndSelection = endSelection;
            this.temporaryFilesPath = temporaryFilesPath;
        }

        public override string ToString()
        {
            return BeginSelection + ", " + EndSelection;
        }
    }
}

namespace csutil.RowComp
{
    public class ExampleRow : IRow<ExampleRow>
    {
        public string ID1 { get; set; }
        public int ID2 { get; set; }
        public string Value { get; set; }

        public ExampleRow(params string[] cols)
        {
            ID1 = cols[0];
            ID2 = int.Parse(cols[1]);
            Value = cols[2];
        }

        public static ExampleRow FromString(string s) => new ExampleRow(s.Split(';'));

        public bool IsSame(ExampleRow row) => ID1 == row.ID1 && ID2 == row.ID2;

        public bool Equals(ExampleRow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID1 == other.ID1 && ID2 == other.ID2 && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ExampleRow) obj);
        }
    }
}
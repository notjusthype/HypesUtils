namespace HypesUtils
{
    internal class Group
    {
        public string? name { get; set; }
        public int? power { get; set; }
        public string? groupColor { get; set; }
        public string? nameColor { get; set; }
        public string? messageColor { get; set; }

        public Group()
        {
        }

        public Group setName(string Name)
        {
            this.name = Name;
            return this;
        }

        public Group setPower(int Power)
        {
            this.power = Power;
            return this;
        }

        public Group setGroupColor(string GroupColor)
        {
            this.groupColor = GroupColor;
            return this;
        }

        public Group setNameColor(string NameColor)
        {
            this.nameColor = NameColor;
            return this;
        }

        public Group setMessageColor(string MessageColor)
        {
            this.messageColor = MessageColor;
            return this;
        }
    }
}
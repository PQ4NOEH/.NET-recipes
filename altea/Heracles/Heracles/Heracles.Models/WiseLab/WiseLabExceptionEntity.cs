namespace Heracles.Models.WiseLab
{
    using System;
    using System.Globalization;

    using Microsoft.WindowsAzure.Storage.Table;

    [CLSCompliant(false)]
    public class WiseLabExceptionEntity : TableEntity
    {
        public WiseLabExceptionEntity(DateTime dateTime)
        {
            this.PartitionKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("WiseLab"));
            this.RowKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dateTime.ToString(CultureInfo.InvariantCulture)));
        }

        public Guid UserId { get; set; }
        public string LanguageFrom { get; set; }
        public string LanguageTo { get; set; }
        public string Origin { get; set; }
        public int Reference { get; set; }
        public string ErrorStack { get; set; }
        public string ErrorMessage { get; set; }
        public string Word { get; set; }
        public string Context { get; set; }
        public string Parent { get; set; }
        public bool Fixed { get; set; }
    }
}

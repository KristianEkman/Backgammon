using System;

namespace Backend.Db
{
    public class Feedback
    {
        public int Id { get; set; }

        public DateTime PostTime{ get; set; }

        public string Text { get; set; }

        public User Sender { get; set; }


    }
}

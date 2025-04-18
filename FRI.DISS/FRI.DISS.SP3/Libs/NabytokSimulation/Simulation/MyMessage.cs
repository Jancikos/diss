using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using OSPABA;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Simulation
{
	public class MyMessage : OSPABA.MessageForm
	{
        public Objednavka? Objednavka { get; set; } = null;
        public Nabytok? Nabytok { get; set; } = null;
        public Pracovisko? Pracovisko { get; set; } = null;
        public Stolar? Stolar { get; set; } = null;

		public MyMessage(OSPABA.Simulation mySim) :
			base(mySim)
		{
		}

		public MyMessage(MyMessage original) :
			base(original)
		{
			// copy() is called in superclass

            // Copy attributes
            Objednavka = original.Objednavka;
            Nabytok = original.Nabytok;
            Pracovisko = original.Pracovisko;
            Stolar = original.Stolar;
		}

		override public MessageForm CreateCopy()
		{
			return new MyMessage(this);
		}

		override protected void Copy(MessageForm message)
		{
			base.Copy(message);
			MyMessage original = (MyMessage)message;
			// Copy attributes
		}
	}
}

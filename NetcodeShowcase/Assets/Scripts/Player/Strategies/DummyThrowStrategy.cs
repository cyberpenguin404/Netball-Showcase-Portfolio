using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Player.Strategies
{
    public class DummyThrowStrategy : IBallThrowingStrategy
    {
        public DummyThrowStrategy()
        {
        }

        public event EventHandler GrabBallRequested;
        public event EventHandler ThrowBallRequested;

        public void Update()
        {
        }

    }
}

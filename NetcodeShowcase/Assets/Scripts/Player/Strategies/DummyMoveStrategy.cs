using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class DummyMoveStrategy : IPlayerMoveStrategy
    {
        public DummyMoveStrategy()
        {
        }

        public event EventHandler CrouchStarted;
        public event EventHandler CrouchEnded;
        public event EventHandler JumpRequested;

        public Vector3 CalculateLookDirection()
        {
            return Vector3.zero;
        }

        public Vector3 CalculateMovement()
        {
            return Vector3.zero;
        }

    }
}

﻿using FIT3077_Pre1975.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class PatientsListIterator : Iterator
    {
        private PatientsList _patientsList;

        // Stores the current traversal position
        private int _position = -1;

        public PatientsListIterator(PatientsList patientsList) 
        {
            _patientsList = patientsList;
        }

        public override object Current()
        {
            return _patientsList.GetPatientAt(_position);
        }

        public override int Key()
        {
            return _position;
        }

        public override bool MoveNext()
        {
            int updatedPosition = _position + 1;
            if (updatedPosition < _patientsList.Count)
            {
                _position = updatedPosition;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Reset()
        {
            _position = 0;
        }
    }
}

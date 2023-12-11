﻿using System.Collections.Generic;
using System.Linq;
using System;

using SimulationModel.Model.Elements;
using SimulationModel.Model.Item;

namespace SimulationModel.Model
{
    public class Model<T> where T: DefaultQueueItem
    {
        private double _currentTime;

        private readonly List<Element<T>> _elements;

        private Action<List<Element<T>>> _additionalAction;

        public Model(List<Element<T>> elements, Action<List<Element<T>>> additionalAction = null)
        {
            _elements = elements;
            _currentTime = 0;

            _additionalAction = additionalAction;
        }

        public void Simulation(double simulationTime, bool finalStats = false, bool stepsStats = false)
        {
            while (_currentTime < simulationTime)
            {
                Element<T> nextElement = _elements.OrderBy(item => item.NextTime()).First();
                _currentTime = nextElement.NextTime();

                foreach (var element in _elements)
                    element.UpdatedCurrentTime(_currentTime);

                if (IsDebug())
                    Console.WriteLine();

                foreach (var element in _elements)
                {
                    if (element.TryFinish())
                    {
                        element.FinishService();
                    }
                    
                }

                if(_additionalAction != null)
                {
                    _additionalAction(_elements);
                }


                if (stepsStats)
                {
                    Console.WriteLine("\n--------------------------- Current Stats -----------------------------");
                    foreach (var element in _elements)
                    {
                        element.PrintStats(false);
                    }
                    Console.WriteLine("-----------------------------------------------------------------------");
                }
            }

            if (finalStats)
            {
                Console.WriteLine("\n========================== Finish Stats ===============================");
                foreach (var element in _elements)
                {
                    element.PrintStats(true);
                }
                Console.WriteLine("========================================================================");
            }
            
        }

        public List<Element<T>> GetElements() { return _elements; }

        private bool IsDebug()
        {
            foreach (var element in _elements)
            {
                if (element.IsDebug)
                    return true;
            }
            return false;
        }
    }
}

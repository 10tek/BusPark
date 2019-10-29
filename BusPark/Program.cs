using BusPark.DataAccess;
using BusPark.Domain;
using BusPark.UI;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace BusPark
{
    class Program
    {
        static void Main(string[] args)
        {
            BusParkUI busParkUI = new BusParkUI();
            busParkUI.Action();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MigrationBundleConsoleAppExample.Models
{
    public class Person
    {
        public int Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }
    }
}
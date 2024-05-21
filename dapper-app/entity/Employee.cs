using System.ComponentModel.DataAnnotations.Schema;

[Table("employee")]
class Employee 
{
        [Column("id")]
        public int Id { get; set;}
        [Column("surname")]
        public string Surname { get; set;}
        [Column("age")]
        public int Age { get; set; }
}
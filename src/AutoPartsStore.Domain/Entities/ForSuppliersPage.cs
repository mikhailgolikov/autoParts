using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPartsStore.Domain.Entities;

/// <summary>
/// Единственная страница "Поставщикам" для этого сайта.
/// </summary>
public class ForSuppliersPage
{
    public static readonly Guid SingletonId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebArchiver.Entities;

namespace WebArchiver.Data
{
    public class PagesConfiguration : IEntityTypeConfiguration<Pages>
    {
        public void Configure(EntityTypeBuilder<Pages> builder)
        {
            builder.Property(p => p.Id).IsRequired();
            builder.Property(p => p.URl).IsRequired();
            builder.Property(p => p.Content).IsRequired();
            builder.Property(p => p.Created).IsRequired();

        }
    }    public class StylesConfiguration : IEntityTypeConfiguration<Styles>
    {
        public void Configure(EntityTypeBuilder<Styles> builder)
        {
            builder.Property(p => p.Id).IsRequired();
            builder.Property(p => p.Content).IsRequired();
            builder.Property(p => p.CreadtedOn).IsRequired();

        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupMuteAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMutedForAll",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMutedForAll",
                table: "Groups");
        }
    }
}

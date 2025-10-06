using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Identity;
using Entities.Entities;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarContato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_agendamento_contato_id",
                table: "agendamento");

            migrationBuilder.CreateIndex(
                name: "IX_agendamento_contato_id",
                table: "agendamento",
                column: "contato_id");

            // ===== SEED INICIAL =====
            var adminId = "11111111-1111-1111-1111-111111111111";
            var securityStamp = "SEED-ADMIN-SECURITY";
            var concurrencyStamp = "SEED-ADMIN-CONCURRENCY";
            var user = new Usuario { Id = adminId, UserName = "administrador", NormalizedUserName = "ADMINISTRADOR", SecurityStamp = securityStamp };
            var hasher = new PasswordHasher<Usuario>();
            var pwdHash = hasher.HashPassword(user, "administrador8485!@");

            migrationBuilder.Sql($@"IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = '{adminId}') BEGIN
INSERT INTO AspNetUsers (Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount,Status)
VALUES ('{adminId}','administrador','ADMINISTRADOR',NULL,NULL,0,'{pwdHash}','{securityStamp}','{concurrencyStamp}',NULL,0,0,0,0,{(int)StatusUsuario.Ativo}); END;
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId='{adminId}' AND RoleId='1') BEGIN INSERT INTO AspNetUserRoles (UserId,RoleId) VALUES ('{adminId}','1'); END;");

            // Contatos (1..11)
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM contato WHERE codg = 1) BEGIN
SET IDENTITY_INSERT contato ON;
INSERT INTO contato (codg,nome,email,telefone) VALUES
(1,'Contato 1','contato1@exemplo.com','11999990001'),
(2,'Contato 2','contato2@exemplo.com','11999990002'),
(3,'Contato 3','contato3@exemplo.com','11999990003'),
(4,'Contato 4','contato4@exemplo.com','11999990004'),
(5,'Contato 5','contato5@exemplo.com','11999990005'),
(6,'Contato 6','contato6@exemplo.com','11999990006'),
(7,'Contato 7','contato7@exemplo.com','11999990007'),
(8,'Contato 8','contato8@exemplo.com','11999990008'),
(9,'Contato 9','contato9@exemplo.com','11999990009'),
(10,'Contato 10','contato10@exemplo.com','11999990010'),
(11,'Contato 11','contato11@exemplo.com','11999990011');
SET IDENTITY_INSERT contato OFF; END;");

            // Agendamentos (15) dentro de 30 dias a partir de 06/10/2025
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM agendamento WHERE codg = 1) BEGIN
SET IDENTITY_INSERT agendamento ON;
INSERT INTO agendamento (codg,data_hora,dscr,contato_id) VALUES
(1,'2025-10-06T09:00:00','Reuniao inicial',1),
(2,'2025-10-07T10:30:00','Follow-up',2),
(3,'2025-10-08T14:15:00','Apresentacao',3),
(4,'2025-10-09T16:45:00','Alinhamento',4),
(5,'2025-10-11T11:00:00','Suporte tecnico',5),
(6,'2025-10-13T15:30:00','Revisao requisitos',6),
(7,'2025-10-16T09:30:00','Planejamento sprint',7),
(8,'2025-10-18T13:00:00','Revisao contrato',8),
(9,'2025-10-21T10:00:00','Call comercial',9),
(10,'2025-10-24T17:00:00','Demonstracao',10),
(11,'2025-10-26T08:30:00','Daily especial',11),
(12,'2025-10-28T14:00:00','Checklist',3),
(13,'2025-10-30T16:00:00','Retrospectiva',5),
(14,'2025-11-02T09:45:00','Onboarding',2),
(15,'2025-11-04T11:15:00','Encerramento',7);
SET IDENTITY_INSERT agendamento OFF; END;");
            // ===== FIM SEED =====
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover dados seed
            migrationBuilder.Sql(@"DELETE FROM agendamento WHERE codg BETWEEN 1 AND 15; DELETE FROM contato WHERE codg BETWEEN 1 AND 11; DELETE FROM AspNetUserRoles WHERE UserId='11111111-1111-1111-1111-111111111111' AND RoleId='1'; DELETE FROM AspNetUsers WHERE Id='11111111-1111-1111-1111-111111111111';");

            migrationBuilder.DropIndex(
                name: "IX_agendamento_contato_id",
                table: "agendamento");

            migrationBuilder.CreateIndex(
                name: "IX_agendamento_contato_id",
                table: "agendamento",
                column: "contato_id",
                unique: true);
        }
    }
}

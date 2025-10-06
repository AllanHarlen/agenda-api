using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Identity;
using Entities.Entities;

#nullable disable

namespace Infraestructure.Migrations
{
    public partial class SeedInicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed usuário administrador + role
            var userId = "11111111-1111-1111-1111-111111111111";
            var securityStamp = "SEED-ADMIN-SECURITY";
            var concurrencyStamp = "SEED-ADMIN-CONCURRENCY";
            var u = new Usuario { Id = userId, UserName = "administrador", NormalizedUserName = "ADMINISTRADOR", SecurityStamp = securityStamp };
            var hasher = new PasswordHasher<Usuario>();
            var pwd = hasher.HashPassword(u, "administrador8485!@");
            migrationBuilder.Sql($@"IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id='{userId}') BEGIN
INSERT INTO AspNetUsers (Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount,Status)
VALUES ('{userId}','administrador','ADMINISTRADOR',NULL,NULL,0,'{pwd}','{securityStamp}','{concurrencyStamp}',NULL,0,0,0,0,0); END;
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId='{userId}' AND RoleId='1') BEGIN INSERT INTO AspNetUserRoles (UserId,RoleId) VALUES ('{userId}','1'); END;");

            // Contatos
            migrationBuilder.Sql(@"SET IDENTITY_INSERT contato ON; IF NOT EXISTS (SELECT 1 FROM contato WHERE codg=1) BEGIN
INSERT INTO contato (codg,nome,email,telefone) VALUES
(1,N'Contato 1','contato1@exemplo.com','11999990001'),
(2,N'Contato 2','contato2@exemplo.com','11999990002'),
(3,N'Contato 3','contato3@exemplo.com','11999990003'),
(4,N'Contato 4','contato4@exemplo.com','11999990004'),
(5,N'Contato 5','contato5@exemplo.com','11999990005'),
(6,N'Contato 6','contato6@exemplo.com','11999990006'),
(7,N'Contato 7','contato7@exemplo.com','11999990007'),
(8,N'Contato 8','contato8@exemplo.com','11999990008'),
(9,N'Contato 9','contato9@exemplo.com','11999990009'),
(10,N'Contato 10','contato10@exemplo.com','11999990010'),
(11,N'Contato 11','contato11@exemplo.com','11999990011'); END; SET IDENTITY_INSERT contato OFF;");

            // Agendamentos (base 2025-10-06)
            migrationBuilder.Sql(@"SET IDENTITY_INSERT agendamento ON; IF NOT EXISTS (SELECT 1 FROM agendamento WHERE codg=1) BEGIN
INSERT INTO agendamento (codg,data_hora,dscr,contato_id) VALUES
(1,'2025-10-06T09:00:00',N'Reunião inicial',1),
(2,'2025-10-07T10:30:00',N'Follow-up',2),
(3,'2025-10-08T14:15:00',N'Apresentação',3),
(4,'2025-10-09T16:45:00',N'Alinhamento',4),
(5,'2025-10-11T11:00:00',N'Suporte técnico',5),
(6,'2025-10-13T15:30:00',N'Revisão de requisitos',6),
(7,'2025-10-16T09:30:00',N'Planejamento sprint',7),
(8,'2025-10-18T13:00:00',N'Revisão contrato',8),
(9,'2025-10-21T10:00:00',N'Call comercial',9),
(10,'2025-10-24T17:00:00',N'Demonstração',10),
(11,'2025-10-26T08:30:00',N'Daily especial',11),
(12,'2025-10-28T14:00:00',N'Checklist',3),
(13,'2025-10-30T16:00:00',N'Retrospectiva',5),
(14,'2025-11-02T09:45:00',N'Onboarding',2),
(15,'2025-11-04T11:15:00',N'Encerramento',7); END; SET IDENTITY_INSERT agendamento OFF;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM agendamento WHERE codg BETWEEN 1 AND 15; DELETE FROM contato WHERE codg BETWEEN 1 AND 11; DELETE FROM AspNetUserRoles WHERE UserId='11111111-1111-1111-1111-111111111111' AND RoleId='1'; DELETE FROM AspNetUsers WHERE Id='11111111-1111-1111-1111-111111111111';");
        }
    }
}

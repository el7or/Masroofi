using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Masroofi.Core.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ATMCardTypes",
                columns: table => new
                {
                    ATMCardTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    TypeNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TypeNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FrontImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValueSql: "((0))"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATMCardTypes", x => x.ATMCardTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CommissionSettings",
                columns: table => new
                {
                    CommissionSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    FromValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ToValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FixedCommission = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PercentageCommission = table.Column<int>(type: "int", nullable: false),
                    VendorFixedCommission = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VendorPercentageCommission = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionSettings", x => x.CommissionSettingId);
                });

            migrationBuilder.CreateTable(
                name: "Governorates",
                columns: table => new
                {
                    GovernorateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GovernorateNameAr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GovernorateNameEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Governorates", x => x.GovernorateId);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Token = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsExpired = table.Column<bool>(type: "bit", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    RoleArabicName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleEnglishName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "SystemPages",
                columns: table => new
                {
                    SystemPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    PageArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageEnglishName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageIndex = table.Column<int>(type: "int", nullable: false),
                    PageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPages", x => x.SystemPageId);
                    table.ForeignKey(
                        name: "FK_SystemPages_SystemPages_ParentPageId",
                        column: x => x.ParentPageId,
                        principalTable: "SystemPages",
                        principalColumn: "SystemPageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Username = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Online = table.Column<bool>(type: "bit", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: true),
                    UserType = table.Column<int>(type: "int", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GovernorateId = table.Column<int>(type: "int", nullable: false),
                    CityNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CityNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityId);
                    table.ForeignKey(
                        name: "FK_Cities_Governorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorates",
                        principalColumn: "GovernorateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemPageActions",
                columns: table => new
                {
                    SystemPageActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ActionArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionEnglishName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionUniqueName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPageActions", x => x.SystemPageActionId);
                    table.ForeignKey(
                        name: "FK_SystemPageActions_SystemPages_SystemPageId",
                        column: x => x.SystemPageId,
                        principalTable: "SystemPages",
                        principalColumn: "SystemPageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleId);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Parents",
                columns: table => new
                {
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "100001, 1"),
                    FullNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Birthdate = table.Column<DateTime>(type: "date", nullable: true),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValueSql: "((0))"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.ParentId);
                    table.ForeignKey(
                        name: "FK_Parents_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    FullNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone1 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsiblePerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GoogleLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValueSql: "((0))"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.VendorId);
                    table.ForeignKey(
                        name: "FK_Vendors_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActionsInRoles",
                columns: table => new
                {
                    ActionsInRolesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemPageActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionsInRoles", x => x.ActionsInRolesId);
                    table.ForeignKey(
                        name: "FK_ActionsInRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActionsInRoles_SystemPageActions_SystemPageActionId",
                        column: x => x.SystemPageActionId,
                        principalTable: "SystemPageActions",
                        principalColumn: "SystemPageActionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentLoginHistories",
                columns: table => new
                {
                    ParentLoginHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentLoginHistories", x => x.ParentLoginHistoryId);
                    table.ForeignKey(
                        name: "FK_ParentLoginHistories_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "ParentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentMobileRegistrations",
                columns: table => new
                {
                    ParentMobileRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Device type (Android,IOS,...)"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentMobileRegistrations", x => x.ParentMobileRegistrationId);
                    table.ForeignKey(
                        name: "FK_ParentMobileRegistrations_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "ParentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentPinCodeHistories",
                columns: table => new
                {
                    ParentPinCodeHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    HistoryType = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentPinCodeHistories", x => x.ParentPinCodeHistoryId);
                    table.ForeignKey(
                        name: "FK_ParentPinCodeHistories_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "ParentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "POSMachines",
                columns: table => new
                {
                    POSMachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    POSModel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    POSNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSMachines", x => x.POSMachineId);
                    table.ForeignKey(
                        name: "FK_POSMachines_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "POSMachineLoginHistories",
                columns: table => new
                {
                    POSMachineLoginHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    POSMachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSMachineLoginHistories", x => x.POSMachineLoginHistoryId);
                    table.ForeignKey(
                        name: "FK_POSMachineLoginHistories_POSMachines_POSMachineId",
                        column: x => x.POSMachineId,
                        principalTable: "POSMachines",
                        principalColumn: "POSMachineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "POSMachinePinCodeHistories",
                columns: table => new
                {
                    POSMachinePinCodeHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    POSMachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    HistoryType = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSMachinePinCodeHistories", x => x.POSMachinePinCodeHistoryId);
                    table.ForeignKey(
                        name: "FK_POSMachinePinCodeHistories_POSMachines_POSMachineId",
                        column: x => x.POSMachineId,
                        principalTable: "POSMachines",
                        principalColumn: "POSMachineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ATMCardHistories",
                columns: table => new
                {
                    ATMCardHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    ATMCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HistoryType = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATMCardHistories", x => x.ATMCardHistoryId);
                });

            migrationBuilder.CreateTable(
                name: "ATMCardTransactions",
                columns: table => new
                {
                    ATMCardTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    ATMCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    POSMachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DetailsAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DetailsEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATMCardTransactions", x => x.ATMCardTransactionId);
                    table.ForeignKey(
                        name: "FK_ATMCardTransactions_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "ParentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ATMCardTransactions_POSMachines_POSMachineId",
                        column: x => x.POSMachineId,
                        principalTable: "POSMachines",
                        principalColumn: "POSMachineId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ATMCardTransactions_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentWalletTransactions",
                columns: table => new
                {
                    ParentWalletTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ATMCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    POSMachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DetailsAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DetailsEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionDataJson = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentWalletTransactions", x => x.ParentWalletTransactionId);
                    table.ForeignKey(
                        name: "FK_ParentWalletTransactions_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "ParentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentWalletTransactions_POSMachines_POSMachineId",
                        column: x => x.POSMachineId,
                        principalTable: "POSMachines",
                        principalColumn: "POSMachineId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentWalletTransactions_Users_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentWalletTransactions_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "POSMachineTransactions",
                columns: table => new
                {
                    POSMachineTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    POSMachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ATMCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DetailsAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DetailsEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSMachineTransactions", x => x.POSMachineTransactionId);
                    table.ForeignKey(
                        name: "FK_POSMachineTransactions_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "ParentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_POSMachineTransactions_POSMachines_POSMachineId",
                        column: x => x.POSMachineId,
                        principalTable: "POSMachines",
                        principalColumn: "POSMachineId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_POSMachineTransactions_POSMachineTransactions_ReferenceTransactionId",
                        column: x => x.ReferenceTransactionId,
                        principalTable: "POSMachineTransactions",
                        principalColumn: "POSMachineTransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_POSMachineTransactions_Users_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_POSMachineTransactions_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionCommissions",
                columns: table => new
                {
                    TransactionCommissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    ParentWalletTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    POSMachineTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    POSMachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    FixedCommissionValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PercentageCommissionValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VendorFixedCommissionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VendorPercentageCommissionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCommissions", x => x.TransactionCommissionId);
                    table.ForeignKey(
                        name: "FK_TransactionCommissions_ParentWalletTransactions_ParentWalletTransactionId",
                        column: x => x.ParentWalletTransactionId,
                        principalTable: "ParentWalletTransactions",
                        principalColumn: "ParentWalletTransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionCommissions_POSMachines_POSMachineId",
                        column: x => x.POSMachineId,
                        principalTable: "POSMachines",
                        principalColumn: "POSMachineId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionCommissions_POSMachineTransactions_POSMachineTransactionId",
                        column: x => x.POSMachineTransactionId,
                        principalTable: "POSMachineTransactions",
                        principalColumn: "POSMachineTransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionCommissions_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sons",
                columns: table => new
                {
                    SonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SonNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SonNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Birthdate = table.Column<DateTime>(type: "date", nullable: false),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValueSql: "((0))"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentATMCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sons", x => x.SonId);
                    table.ForeignKey(
                        name: "FK_Sons_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "ParentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ATMCards",
                columns: table => new
                {
                    ATMCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NEWID())"),
                    SonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ATMCardTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "100001, 1"),
                    ShortNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    CardNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: true),
                    SecurityCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))"),
                    RejectedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATMCards", x => x.ATMCardId);
                    table.ForeignKey(
                        name: "FK_ATMCards_ATMCardTypes_ATMCardTypeId",
                        column: x => x.ATMCardTypeId,
                        principalTable: "ATMCardTypes",
                        principalColumn: "ATMCardTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ATMCards_Sons_SonId",
                        column: x => x.SonId,
                        principalTable: "Sons",
                        principalColumn: "SonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionsInRoles_RoleId",
                table: "ActionsInRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionsInRoles_SystemPageActionId",
                table: "ActionsInRoles",
                column: "SystemPageActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ATMCardHistories_ATMCardId",
                table: "ATMCardHistories",
                column: "ATMCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ATMCards_ATMCardTypeId",
                table: "ATMCards",
                column: "ATMCardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ATMCards_SonId",
                table: "ATMCards",
                column: "SonId");

            migrationBuilder.CreateIndex(
                name: "IX_ATMCardTransactions_ATMCardId",
                table: "ATMCardTransactions",
                column: "ATMCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ATMCardTransactions_ParentId",
                table: "ATMCardTransactions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ATMCardTransactions_POSMachineId",
                table: "ATMCardTransactions",
                column: "POSMachineId");

            migrationBuilder.CreateIndex(
                name: "IX_ATMCardTransactions_SonId",
                table: "ATMCardTransactions",
                column: "SonId");

            migrationBuilder.CreateIndex(
                name: "IX_ATMCardTransactions_VendorId",
                table: "ATMCardTransactions",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_GovernorateId",
                table: "Cities",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentLoginHistories_ParentId",
                table: "ParentLoginHistories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentMobileRegistrations_ParentId",
                table: "ParentMobileRegistrations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentPinCodeHistories_ParentId",
                table: "ParentPinCodeHistories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Parents_CityId",
                table: "Parents",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentWalletTransactions_AdminUserId",
                table: "ParentWalletTransactions",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentWalletTransactions_ATMCardId",
                table: "ParentWalletTransactions",
                column: "ATMCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentWalletTransactions_ParentId",
                table: "ParentWalletTransactions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentWalletTransactions_POSMachineId",
                table: "ParentWalletTransactions",
                column: "POSMachineId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentWalletTransactions_SonId",
                table: "ParentWalletTransactions",
                column: "SonId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentWalletTransactions_VendorId",
                table: "ParentWalletTransactions",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_POSMachineLoginHistories_POSMachineId",
                table: "POSMachineLoginHistories",
                column: "POSMachineId");

            migrationBuilder.CreateIndex(
                name: "IX_POSMachinePinCodeHistories_POSMachineId",
                table: "POSMachinePinCodeHistories",
                column: "POSMachineId");

            migrationBuilder.CreateIndex(
                name: "IX_POSMachines_Username",
                table: "POSMachines",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_POSMachines_VendorId",
                table: "POSMachines",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_POSMachineTransactions_AdminUserId",
                table: "POSMachineTransactions",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_POSMachineTransactions_ATMCardId",
                table: "POSMachineTransactions",
                column: "ATMCardId");

            migrationBuilder.CreateIndex(
                name: "IX_POSMachineTransactions_ParentId",
                table: "POSMachineTransactions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_POSMachineTransactions_POSMachineId",
                table: "POSMachineTransactions",
                column: "POSMachineId");

            migrationBuilder.CreateIndex(
                name: "IX_POSMachineTransactions_ReferenceTransactionId",
                table: "POSMachineTransactions",
                column: "ReferenceTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_POSMachineTransactions_SonId",
                table: "POSMachineTransactions",
                column: "SonId");

            migrationBuilder.CreateIndex(
                name: "IX_POSMachineTransactions_VendorId",
                table: "POSMachineTransactions",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sons_CurrentATMCardId",
                table: "Sons",
                column: "CurrentATMCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Sons_ParentId",
                table: "Sons",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPageActions_SystemPageId",
                table: "SystemPageActions",
                column: "SystemPageId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPages_ParentPageId",
                table: "SystemPages",
                column: "ParentPageId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCommissions_ParentWalletTransactionId",
                table: "TransactionCommissions",
                column: "ParentWalletTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCommissions_POSMachineId",
                table: "TransactionCommissions",
                column: "POSMachineId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCommissions_POSMachineTransactionId",
                table: "TransactionCommissions",
                column: "POSMachineTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCommissions_VendorId",
                table: "TransactionCommissions",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CityId",
                table: "Vendors",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ATMCardHistories_ATMCards_ATMCardId",
                table: "ATMCardHistories",
                column: "ATMCardId",
                principalTable: "ATMCards",
                principalColumn: "ATMCardId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ATMCardTransactions_ATMCards_ATMCardId",
                table: "ATMCardTransactions",
                column: "ATMCardId",
                principalTable: "ATMCards",
                principalColumn: "ATMCardId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ATMCardTransactions_Sons_SonId",
                table: "ATMCardTransactions",
                column: "SonId",
                principalTable: "Sons",
                principalColumn: "SonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentWalletTransactions_ATMCards_ATMCardId",
                table: "ParentWalletTransactions",
                column: "ATMCardId",
                principalTable: "ATMCards",
                principalColumn: "ATMCardId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentWalletTransactions_Sons_SonId",
                table: "ParentWalletTransactions",
                column: "SonId",
                principalTable: "Sons",
                principalColumn: "SonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_POSMachineTransactions_ATMCards_ATMCardId",
                table: "POSMachineTransactions",
                column: "ATMCardId",
                principalTable: "ATMCards",
                principalColumn: "ATMCardId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_POSMachineTransactions_Sons_SonId",
                table: "POSMachineTransactions",
                column: "SonId",
                principalTable: "Sons",
                principalColumn: "SonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sons_ATMCards_CurrentATMCardId",
                table: "Sons",
                column: "CurrentATMCardId",
                principalTable: "ATMCards",
                principalColumn: "ATMCardId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sons_ATMCards_CurrentATMCardId",
                table: "Sons");

            migrationBuilder.DropTable(
                name: "ActionsInRoles");

            migrationBuilder.DropTable(
                name: "ATMCardHistories");

            migrationBuilder.DropTable(
                name: "ATMCardTransactions");

            migrationBuilder.DropTable(
                name: "CommissionSettings");

            migrationBuilder.DropTable(
                name: "ParentLoginHistories");

            migrationBuilder.DropTable(
                name: "ParentMobileRegistrations");

            migrationBuilder.DropTable(
                name: "ParentPinCodeHistories");

            migrationBuilder.DropTable(
                name: "POSMachineLoginHistories");

            migrationBuilder.DropTable(
                name: "POSMachinePinCodeHistories");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "TransactionCommissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "SystemPageActions");

            migrationBuilder.DropTable(
                name: "ParentWalletTransactions");

            migrationBuilder.DropTable(
                name: "POSMachineTransactions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "SystemPages");

            migrationBuilder.DropTable(
                name: "POSMachines");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "ATMCards");

            migrationBuilder.DropTable(
                name: "ATMCardTypes");

            migrationBuilder.DropTable(
                name: "Sons");

            migrationBuilder.DropTable(
                name: "Parents");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Governorates");
        }
    }
}

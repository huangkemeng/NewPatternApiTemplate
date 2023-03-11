using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RenameMe.Api.Infrastructure.EfCore.Entities.Bases;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using PropertyBuilder = Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder;

namespace RenameMe.Api.Infrastructure.Bases
{
    public static class EntityConfigureExtension
    {
        private static Assembly? mappingAssembly;
        public static void AutoConfigure<T>(this EntityTypeBuilder<T> builder) where T : class
        {
            var entityType = typeof(T);
            builder.ToTable(entityType.Name);
            var entityProperties = entityType.GetProperties().Where(e => !e.GetAccessors()?[0].IsVirtual ?? false).ToArray();
            var typeMaps = CreateClrTypeToSqlTypeMaps();
            var propMethodInfo = typeof(EntityTypeBuilder<T>).GetMethod(nameof(EntityTypeBuilder<T>.Property), genericParameterCount: 0, new Type[] { typeof(Type), typeof(string) })!;
            var propertyBuilderType = typeof(RelationalPropertyBuilderExtensions);
            var hasColumnNameType = propertyBuilderType.GetMethod(nameof(RelationalPropertyBuilderExtensions.HasColumnName), genericParameterCount: 0, new Type[] { typeof(PropertyBuilder), typeof(string) });
            PropertyBuilder? propBuilder = null;

            foreach (var entityProperty in entityProperties)
            {
                if (typeMaps.ContainsKey(entityProperty.PropertyType))
                {
                    propBuilder = (PropertyBuilder?)propMethodInfo.Invoke(builder, new object[] { entityProperty.PropertyType, entityProperty.Name });
                    if (propBuilder != null)
                    {
                        var dbType = typeMaps[entityProperty.PropertyType];
                        propBuilder = RelationalPropertyBuilderExtensions.HasColumnName(propBuilder, entityProperty.Name);
                        propBuilder = RelationalPropertyBuilderExtensions.HasColumnType(propBuilder, dbType.ToString());
                        if (entityProperty.PropertyType.IsGenericType && entityProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var isRequiredType = typeof(PropertyBuilder).GetMethod(nameof(PropertyBuilder.IsRequired), new Type[] { typeof(bool) });
                            propBuilder = (PropertyBuilder?)isRequiredType?.Invoke(propBuilder, new object[] { false });
                        }
                        if (entityProperty.PropertyType == typeof(string))
                        {
                            var hasMaxLengthType = typeof(PropertyBuilder).GetMethod(nameof(PropertyBuilder.HasMaxLength), new Type[] { typeof(int) });
                            propBuilder = (PropertyBuilder?)hasMaxLengthType?.Invoke(propBuilder, new object[] { 100 });
                        }
                    }
                }
            }
        }

        private static Dictionary<Type, SqlDbType> CreateClrTypeToSqlTypeMaps()
        {
            return new Dictionary<Type, SqlDbType>
            {
                {typeof (Boolean), SqlDbType.Bit},
                {typeof (Boolean?), SqlDbType.Bit},
                {typeof (Byte), SqlDbType.TinyInt},
                {typeof (Byte?), SqlDbType.TinyInt},
                {typeof (String), SqlDbType.NVarChar},
                {typeof (DateTime), SqlDbType.DateTime},
                {typeof (DateTime?), SqlDbType.DateTime},
                {typeof (Int16), SqlDbType.SmallInt},
                {typeof (Int16?), SqlDbType.SmallInt},
                {typeof (Int32), SqlDbType.Int},
                {typeof (Int32?), SqlDbType.Int},
                {typeof (Int64), SqlDbType.BigInt},
                {typeof (Int64?), SqlDbType.BigInt},
                {typeof (Decimal), SqlDbType.Decimal},
                {typeof (Decimal?), SqlDbType.Decimal},
                {typeof (Double), SqlDbType.Float},
                {typeof (Double?), SqlDbType.Float},
                {typeof (Single), SqlDbType.Real},
                {typeof (Single?), SqlDbType.Real},
                {typeof (TimeSpan), SqlDbType.Time},
                {typeof (Guid), SqlDbType.UniqueIdentifier},
                {typeof (Guid?), SqlDbType.UniqueIdentifier},
                {typeof (Byte[]), SqlDbType.Binary},
                {typeof (Byte?[]), SqlDbType.Binary},
                {typeof (Char[]), SqlDbType.Char},
                {typeof (Char?[]), SqlDbType.Char}
            };
        }

        public static void LoadFromEntityConfigure(this ModelBuilder modelBuilder)
        {
            if (mappingAssembly == null)
            {
                mappingAssembly = CreateConfigureEntityAssembly();
            }
            if (mappingAssembly != null)
            {
                modelBuilder.ApplyConfigurationsFromAssembly(mappingAssembly!);
            }
        }

        private static Assembly? CreateConfigureEntityAssembly()
        {
            var idbEntityType = typeof(IEfDbEntity<>);
            var idbEntityAssembly = idbEntityType.Assembly;
            var dbEntityTypes = idbEntityAssembly
                ?.ExportedTypes
                .Where(e => e.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == idbEntityType) && e.IsClass && !e.IsAbstract)
                .ToArray();
            if (dbEntityTypes != null && dbEntityTypes.Any())
            {
                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("EfCoreDbEntityMappingAssembly"), AssemblyBuilderAccess.Run);
                var moduleBuilder = assemblyBuilder.DefineDynamicModule("EfCoreEntityMappingModule");
                var ientityTypeConfigurationType = typeof(IEntityTypeConfiguration<>);
                var entityTypeBuilderType = typeof(EntityTypeBuilder<>);
                Type? createdType = null;
                foreach (var dbEntityType in dbEntityTypes)
                {
                    var dbContextProp = dbEntityType.GetMethod(nameof(IEfDbEntity<IEntityPrimary>.EntityConfigure))!;
                    var typeBuilder = moduleBuilder.DefineType($"{dbEntityType.Name}EfCoreEntityMapping", TypeAttributes.Public | TypeAttributes.Class);
                    var ientityTypeConfigurationGenericType = ientityTypeConfigurationType.MakeGenericType(dbEntityType);
                    typeBuilder.AddInterfaceImplementation(ientityTypeConfigurationGenericType);
                    var methodBuilder = typeBuilder.DefineMethod("Configure", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { entityTypeBuilderType.MakeGenericType(dbEntityType) });
                    methodBuilder.DefineParameter(1, ParameterAttributes.None, "builder");
                    var il = methodBuilder.GetILGenerator();
                    var methodinfo = dbEntityType.GetMethod("EntityConfigure", BindingFlags.Static | BindingFlags.Public)!;
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, methodinfo);
                    il.Emit(OpCodes.Ret);
                    createdType = typeBuilder.CreateType();
                }
                return createdType?.Assembly;
            }
            return null;
        }
    }
}

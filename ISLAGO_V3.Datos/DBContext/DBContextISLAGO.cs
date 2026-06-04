using System;
using System.Collections.Generic;
using ISLAGO_V3.Entidad.Models;
using Microsoft.EntityFrameworkCore;

namespace ISLAGO_V3.Datos;

public partial class DBContextISLAGO : DbContext
{
    public DBContextISLAGO()
    {
    }

    public DBContextISLAGO(DbContextOptions<DBContextISLAGO> options)
        : base(options)
    {
    }

    public virtual DbSet<Articulo> Articulos { get; set; }

    public virtual DbSet<Articulocategorium> Articulocategoria { get; set; }

    public virtual DbSet<Articuloimagen> Articuloimagens { get; set; }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<Codigo2fa> Codigo2fas { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Descuento> Descuentos { get; set; }

    public virtual DbSet<DetalleCompra> DetalleCompras { get; set; }

    public virtual DbSet<DetalleFactura> DetalleFacturas { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<DetalleProforma> DetalleProformas { get; set; }

    public virtual DbSet<Devolucion> Devolucions { get; set; }

    public virtual DbSet<Entrega> Entregas { get; set; }

    public virtual DbSet<Estadopedido> Estadopedidos { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<FormaPago> FormaPagos { get; set; }

    public virtual DbSet<Historialestadopedido> Historialestadopedidos { get; set; }

    public virtual DbSet<Imagen> Imagens { get; set; }

    public virtual DbSet<MovimientoInventario> MovimientoInventarios { get; set; }

    public virtual DbSet<Notificacion> Notificacions { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Proforma> Proformas { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<RecuperacionCuentum> RecuperacionCuenta { get; set; }

    public virtual DbSet<Reporte> Reportes { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<SesionUsuario> SesionUsuarios { get; set; }

    public virtual DbSet<TipoPersona> TipoPersonas { get; set; }

    public virtual DbSet<Umedidum> Umedida { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Usuario2fauth> Usuario2fauths { get; set; }

    public virtual DbSet<UsuarioRol> UsuarioRols { get; set; }

    public virtual DbSet<Usuarioimagen> Usuarioimagens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("articulo_pkey");

            entity.ToTable("articulo");

            entity.HasIndex(e => e.Sku, "articulo_sku_unique").IsUnique();

            entity.HasIndex(e => e.Nombre, "idx_articulo_nombre");

            entity.HasIndex(e => e.Sku, "idx_articulo_sku");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.EsServicio)
                .HasDefaultValue(false)
                .HasColumnName("es_servicio");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Idproveedor).HasColumnName("idproveedor");
            entity.Property(e => e.Idumedida).HasColumnName("idumedida");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .HasColumnName("marca");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.PermiteDecimal)
                .HasDefaultValue(false)
                .HasColumnName("permite_decimal");
            entity.Property(e => e.PermiteDescuento)
                .HasDefaultValue(true)
                .HasColumnName("permite_descuento");
            entity.Property(e => e.PorcentajeGananciaMayorista)
                .HasPrecision(5, 2)
                .HasColumnName("porcentaje_ganancia_mayorista");
            entity.Property(e => e.PorcentajeGananciaMinorista)
                .HasPrecision(5, 2)
                .HasColumnName("porcentaje_ganancia_minorista");
            entity.Property(e => e.PrecioCompra)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("precio_compra");
            entity.Property(e => e.PrecioVentaMayorista)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("precio_venta_mayorista");
            entity.Property(e => e.PrecioVentaMinorista)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("precio_venta_minorista");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.Stock)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("stock");
            entity.Property(e => e.StockMinimo)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("stock_minimo");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(150)
                .HasColumnName("ubicacion");

            entity.HasOne(d => d.IdproveedorNavigation).WithMany(p => p.Articulos)
                .HasForeignKey(d => d.Idproveedor)
                .HasConstraintName("articulo_idproveedor_fkey");

            entity.HasOne(d => d.IdumedidaNavigation).WithMany(p => p.Articulos)
                .HasForeignKey(d => d.Idumedida)
                .HasConstraintName("articulo_idumedida_fkey");
        });

        modelBuilder.Entity<Articulocategorium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("articulocategoria_pkey");

            entity.ToTable("articulocategoria");

            entity.HasIndex(e => new { e.Idarticulo, e.Idcategoria }, "uq_articulo_categoria").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Idcategoria).HasColumnName("idcategoria");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.Articulocategoria)
                .HasForeignKey(d => d.Idarticulo)
                .HasConstraintName("articulocategoria_idarticulo_fkey");

            entity.HasOne(d => d.IdcategoriaNavigation).WithMany(p => p.Articulocategoria)
                .HasForeignKey(d => d.Idcategoria)
                .HasConstraintName("articulocategoria_idcategoria_fkey");
        });

        modelBuilder.Entity<Articuloimagen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("articuloimagen_pkey");

            entity.ToTable("articuloimagen");

            entity.HasIndex(e => e.Idarticulo, "idx_articuloimagen_articulo");

            entity.HasIndex(e => e.Idimagen, "idx_articuloimagen_imagen");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EsPrincipal)
                .HasDefaultValue(false)
                .HasColumnName("es_principal");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Idimagen).HasColumnName("idimagen");
            entity.Property(e => e.Orden)
                .HasDefaultValue(0)
                .HasColumnName("orden");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.Articuloimagens)
                .HasForeignKey(d => d.Idarticulo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("articuloimagen_idarticulo_fkey");

            entity.HasOne(d => d.IdimagenNavigation).WithMany(p => p.Articuloimagens)
                .HasForeignKey(d => d.Idimagen)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("articuloimagen_idimagen_fkey");
        });

        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categoria_pkey");

            entity.ToTable("categoria");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Idpadre).HasColumnName("idpadre");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdpadreNavigation).WithMany(p => p.InverseIdpadreNavigation)
                .HasForeignKey(d => d.Idpadre)
                .HasConstraintName("categoria_idpadre_fkey");
        });

        modelBuilder.Entity<Codigo2fa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("codigo2fa_pkey");

            entity.ToTable("codigo2fa");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Codigo)
                .HasMaxLength(10)
                .HasColumnName("codigo");
            entity.Property(e => e.Expiracion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expiracion");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Usado)
                .HasDefaultValue(false)
                .HasColumnName("usado");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Codigo2fas)
                .HasForeignKey(d => d.Idusuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("codigo2fa_idusuario_fkey");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("compra_pkey");

            entity.ToTable("compra");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'COMPLETADA'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Idproveedor).HasColumnName("idproveedor");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.IdproveedorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.Idproveedor)
                .HasConstraintName("compra_idproveedor_fkey");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("compra_idusuario_fkey");
        });

        modelBuilder.Entity<Descuento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("descuentos_pkey");

            entity.ToTable("descuentos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Idcliente).HasColumnName("idcliente");
            entity.Property(e => e.Porcentaje)
                .HasPrecision(5, 2)
                .HasColumnName("porcentaje");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.Descuentos)
                .HasForeignKey(d => d.Idarticulo)
                .HasConstraintName("descuentos_idarticulo_fkey");

            entity.HasOne(d => d.IdclienteNavigation).WithMany(p => p.Descuentos)
                .HasForeignKey(d => d.Idcliente)
                .HasConstraintName("descuentos_idcliente_fkey");
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("detalle_compra_pkey");

            entity.ToTable("detalle_compra");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Idcompra).HasColumnName("idcompra");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.Idarticulo)
                .HasConstraintName("detalle_compra_idarticulo_fkey");

            entity.HasOne(d => d.IdcompraNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.Idcompra)
                .HasConstraintName("detalle_compra_idcompra_fkey");
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("detalle_factura_pkey");

            entity.ToTable("detalle_factura");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Idfactura).HasColumnName("idfactura");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.Idarticulo)
                .HasConstraintName("detalle_factura_idarticulo_fkey");

            entity.HasOne(d => d.IdfacturaNavigation).WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.Idfactura)
                .HasConstraintName("detalle_factura_idfactura_fkey");
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("detalle_pedido_pkey");

            entity.ToTable("detalle_pedido");

            entity.HasIndex(e => e.Idarticulo, "idx_detallepedido_articulo");

            entity.HasIndex(e => e.Idpedido, "idx_detallepedido_pedido");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.CostoUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("costo_unitario");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Idpedido).HasColumnName("idpedido");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.Idarticulo)
                .HasConstraintName("detalle_pedido_idarticulo_fkey");

            entity.HasOne(d => d.IdpedidoNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.Idpedido)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("detalle_pedido_idpedido_fkey");
        });

        modelBuilder.Entity<DetalleProforma>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("detalle_proforma_pkey");

            entity.ToTable("detalle_proforma");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Idproforma).HasColumnName("idproforma");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.DetalleProformas)
                .HasForeignKey(d => d.Idarticulo)
                .HasConstraintName("detalle_proforma_idarticulo_fkey");

            entity.HasOne(d => d.IdproformaNavigation).WithMany(p => p.DetalleProformas)
                .HasForeignKey(d => d.Idproforma)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("detalle_proforma_idproforma_fkey");
        });

        modelBuilder.Entity<Devolucion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("devolucion_pkey");

            entity.ToTable("devolucion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasColumnName("estado");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Idpedido).HasColumnName("idpedido");
            entity.Property(e => e.Motivo).HasColumnName("motivo");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.Devolucions)
                .HasForeignKey(d => d.Idarticulo)
                .HasConstraintName("devolucion_idarticulo_fkey");

            entity.HasOne(d => d.IdpedidoNavigation).WithMany(p => p.Devolucions)
                .HasForeignKey(d => d.Idpedido)
                .HasConstraintName("devolucion_idpedido_fkey");
        });

        modelBuilder.Entity<Entrega>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("entrega_pkey");

            entity.ToTable("entrega");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaReal)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_real");
            entity.Property(e => e.IdimagenEvidencia).HasColumnName("idimagen_evidencia");
            entity.Property(e => e.Idpedido).HasColumnName("idpedido");

            entity.HasOne(d => d.IdimagenEvidenciaNavigation).WithMany(p => p.Entregas)
                .HasForeignKey(d => d.IdimagenEvidencia)
                .HasConstraintName("fk_entrega_imagen");

            entity.HasOne(d => d.IdpedidoNavigation).WithMany(p => p.Entregas)
                .HasForeignKey(d => d.Idpedido)
                .HasConstraintName("entrega_idpedido_fkey");
        });

        modelBuilder.Entity<Estadopedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estadopedido_pkey");

            entity.ToTable("estadopedido");

            entity.HasIndex(e => e.Nombre, "estadopedido_nombre_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("factura_pkey");

            entity.ToTable("factura");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Idimagen).HasColumnName("idimagen");
            entity.Property(e => e.Idpedido).HasColumnName("idpedido");
            entity.Property(e => e.Numero)
                .HasMaxLength(50)
                .HasColumnName("numero");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.IdimagenNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.Idimagen)
                .HasConstraintName("fk_factura_imagen");

            entity.HasOne(d => d.IdpedidoNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.Idpedido)
                .HasConstraintName("factura_idpedido_fkey");
        });

        modelBuilder.Entity<FormaPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("forma_pago_pkey");

            entity.ToTable("forma_pago");

            entity.HasIndex(e => e.Nombre, "forma_pago_nombre_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Historialestadopedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("historialestadopedido_pkey");

            entity.ToTable("historialestadopedido");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Idestadopedido).HasColumnName("idestadopedido");
            entity.Property(e => e.Idpedido).HasColumnName("idpedido");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");

            entity.HasOne(d => d.IdestadopedidoNavigation).WithMany(p => p.Historialestadopedidos)
                .HasForeignKey(d => d.Idestadopedido)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("historialestadopedido_idestadopedido_fkey");

            entity.HasOne(d => d.IdpedidoNavigation).WithMany(p => p.Historialestadopedidos)
                .HasForeignKey(d => d.Idpedido)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("historialestadopedido_idpedido_fkey");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Historialestadopedidos)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("historial_idusuario_fkey");
        });

        modelBuilder.Entity<Imagen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("imagen_pkey");

            entity.ToTable("imagen");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Fechaeditada)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fechaeditada");
            entity.Property(e => e.Fechapublicada)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fechapublicada");
            entity.Property(e => e.Hash).HasColumnName("hash");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.Ruta).HasColumnName("ruta");
            entity.Property(e => e.Tamaño).HasColumnName("tamaño");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("movimiento_inventario_pkey");

            entity.ToTable("movimiento_inventario");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Idcompra).HasColumnName("idcompra");
            entity.Property(e => e.Idpedido).HasColumnName("idpedido");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Motivo).HasColumnName("motivo");
            entity.Property(e => e.StockAnterior)
                .HasPrecision(10, 2)
                .HasColumnName("stock_anterior");
            entity.Property(e => e.StockNuevo)
                .HasPrecision(10, 2)
                .HasColumnName("stock_nuevo");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.Idarticulo)
                .HasConstraintName("movimiento_inventario_idarticulo_fkey");

            entity.HasOne(d => d.IdcompraNavigation).WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.Idcompra)
                .HasConstraintName("movimiento_idcompra_fkey");

            entity.HasOne(d => d.IdpedidoNavigation).WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.Idpedido)
                .HasConstraintName("movimiento_idpedido_fkey");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("movimiento_idusuario_fkey");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notificacion_pkey");

            entity.ToTable("notificacion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Leido)
                .HasDefaultValue(false)
                .HasColumnName("leido");
            entity.Property(e => e.Mensaje).HasColumnName("mensaje");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Notificacions)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("notificacion_idusuario_fkey");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pagos_pkey");

            entity.ToTable("pagos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comprobante).HasColumnName("comprobante");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Idformapago).HasColumnName("idformapago");
            entity.Property(e => e.Idpedido).HasColumnName("idpedido");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Monto)
                .HasPrecision(10, 2)
                .HasColumnName("monto");
            entity.Property(e => e.Referencia)
                .HasMaxLength(100)
                .HasColumnName("referencia");

            entity.HasOne(d => d.IdformapagoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.Idformapago)
                .HasConstraintName("pagos_idformapago_fkey");

            entity.HasOne(d => d.IdpedidoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.Idpedido)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("pagos_idpedido_fkey");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("pagos_idusuario_fkey");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pedido_pkey");

            entity.ToTable("pedido");

            entity.HasIndex(e => e.Idcliente, "idx_pedido_cliente");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Deuda)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("deuda");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaEntrega)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_entrega");
            entity.Property(e => e.Idcliente).HasColumnName("idcliente");
            entity.Property(e => e.Idestado).HasColumnName("idestado");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.NombreClienteTemp)
                .HasMaxLength(100)
                .HasColumnName("nombre_cliente_temp");
            entity.Property(e => e.Prioridad)
                .HasMaxLength(10)
                .HasColumnName("prioridad");
            entity.Property(e => e.TelefonoTemp)
                .HasMaxLength(25)
                .HasColumnName("telefono_temp");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");
            entity.Property(e => e.TotalPagado)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("total_pagado");

            entity.HasOne(d => d.IdclienteNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.Idcliente)
                .HasConstraintName("pedido_idcliente_fkey");

            entity.HasOne(d => d.IdestadoNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.Idestado)
                .HasConstraintName("pedido_idestado_fkey");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("pedido_idusuario_fkey");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("persona_pkey");

            entity.ToTable("persona");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(150)
                .HasColumnName("apellidos");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.IdtipoPersona).HasColumnName("idtipo_persona");
            entity.Property(e => e.Nombres)
                .HasMaxLength(150)
                .HasColumnName("nombres");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdtipoPersonaNavigation).WithMany(p => p.Personas)
                .HasForeignKey(d => d.IdtipoPersona)
                .HasConstraintName("fk_tipopersona");
        });

        modelBuilder.Entity<Proforma>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proforma_pkey");

            entity.ToTable("proforma");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Idcliente).HasColumnName("idcliente");
            entity.Property(e => e.Idimagen).HasColumnName("idimagen");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.NombreClienteTemp)
                .HasMaxLength(100)
                .HasColumnName("nombre_cliente_temp");
            entity.Property(e => e.TelefonoTemp)
                .HasMaxLength(25)
                .HasColumnName("telefono_temp");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.IdclienteNavigation).WithMany(p => p.Proformas)
                .HasForeignKey(d => d.Idcliente)
                .HasConstraintName("proforma_idcliente_fkey");

            entity.HasOne(d => d.IdimagenNavigation).WithMany(p => p.Proformas)
                .HasForeignKey(d => d.Idimagen)
                .HasConstraintName("fk_proforma_imagen");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Proformas)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("proforma_idusuario_fkey");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proveedor_pkey");

            entity.ToTable("proveedor");

            entity.HasIndex(e => e.Idpersona, "proveedor_idpersona_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Idpersona).HasColumnName("idpersona");

            entity.HasOne(d => d.IdpersonaNavigation).WithOne(p => p.Proveedor)
                .HasForeignKey<Proveedor>(d => d.Idpersona)
                .HasConstraintName("proveedor_idpersona_fkey");
        });

        modelBuilder.Entity<RecuperacionCuentum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("recuperacion_cuenta_pkey");

            entity.ToTable("recuperacion_cuenta");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaExpiracion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_expiracion");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.Usado)
                .HasDefaultValue(false)
                .HasColumnName("usado");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.RecuperacionCuenta)
                .HasForeignKey(d => d.Idusuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("recuperacion_cuenta_idusuario_fkey");
        });

        modelBuilder.Entity<Reporte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reporte_pkey");

            entity.ToTable("reporte");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaActualizado)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_actualizado");
            entity.Property(e => e.FechaGenerado)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_generado");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Ruta).HasColumnName("ruta");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Reportes)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("reporte_idusuario_fkey");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rol_pkey");

            entity.ToTable("rol");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(60)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<SesionUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sesion_usuario_pkey");

            entity.ToTable("sesion_usuario");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.FechaExpiracion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_expiracion");
            entity.Property(e => e.FechaInicio)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Ip)
                .HasMaxLength(70)
                .HasColumnName("ip");
            entity.Property(e => e.Token).HasColumnName("token");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.SesionUsuarios)
                .HasForeignKey(d => d.Idusuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("sesion_usuario_idusuario_fkey");
        });

        modelBuilder.Entity<TipoPersona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipo_persona_pkey");

            entity.ToTable("tipo_persona");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Umedidum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("umedida_pkey");

            entity.ToTable("umedida");

            entity.HasIndex(e => e.Nombre, "umedida_nombre_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuario_pkey");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Email, "usuario_email_key").IsUnique();

            entity.HasIndex(e => e.Idpersona, "usuario_idpersona_key").IsUnique();

            entity.HasIndex(e => e.Usarname, "usuario_usarname_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Bloqueado)
                .HasDefaultValue(false)
                .HasColumnName("bloqueado");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Idpersona).HasColumnName("idpersona");
            entity.Property(e => e.IntentosFallidos)
                .HasDefaultValue(0)
                .HasColumnName("intentos_fallidos");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.UltimoLogin)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("ultimo_login");
            entity.Property(e => e.Usarname)
                .HasMaxLength(60)
                .HasColumnName("usarname");

            entity.HasOne(d => d.IdpersonaNavigation).WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(d => d.Idpersona)
                .HasConstraintName("usuario_idpersona_fkey");
        });

        modelBuilder.Entity<Usuario2fauth>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuario2fauth_pkey");

            entity.ToTable("usuario2fauth");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(false)
                .HasColumnName("activo");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Secreto).HasColumnName("secreto");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Usuario2fauths)
                .HasForeignKey(d => d.Idusuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usuario2fauth_idusuario_fkey");
        });

        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuario_rol_pkey");

            entity.ToTable("usuario_rol");

            entity.HasIndex(e => e.Idusuario, "idx_usuario_rol_usuario");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Idrol).HasColumnName("idrol");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");

            entity.HasOne(d => d.IdrolNavigation).WithMany(p => p.UsuarioRols)
                .HasForeignKey(d => d.Idrol)
                .HasConstraintName("usuario_rol_idrol_fkey");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.UsuarioRols)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("usuario_rol_idusuario_fkey");
        });

        modelBuilder.Entity<Usuarioimagen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuarioimagen_pkey");

            entity.ToTable("usuarioimagen");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EsPrincipal)
                .HasDefaultValue(false)
                .HasColumnName("es_principal");
            entity.Property(e => e.Idimagen).HasColumnName("idimagen");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Orden)
                .HasDefaultValue(0)
                .HasColumnName("orden");

            entity.HasOne(d => d.IdimagenNavigation).WithMany(p => p.Usuarioimagens)
                .HasForeignKey(d => d.Idimagen)
                .HasConstraintName("fk_usuarioimagen_imagen");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Usuarioimagens)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("fk_usuarioimagen_usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

create database bd_ecommerce
use bd_ecommerce


create table provincias(
id_provincia int primary key identity,
nombre_provincia varchar(60))

go

insert into provincias values('Santa Fe')

create table ciudades(
codigo_postal int primary key,
nombre varchar(60),
id_provincia int foreign key references provincias(id_provincia))

go

insert into ciudades values('2000','Rosario','1')

create table categorias(
id_categoria int primary key identity,
nombre_categoria varchar(60),
activo bit default 1,
fecha_registro datetime default getdate())

go

insert into categorias(nombre_categoria) values('Tecnologia')

create table marca(
id_marca int primary key identity,
nombre_marca varchar(60),
activo bit default 1,
fecha_registro datetime default getdate())

go

create table productos(
id_producto int primary key identity,
nombre varchar(60),
descripcion varchar(200),
precio decimal(18,2),
stock int,
rutaImagen varchar(100),
nombreImagen varchar(100),
activo bit default 1,
fecha_registro datetime default getdate(),
id_marca int foreign key references marca(id_marca),
id_categoria int foreign key references categorias(id_categoria))

go

create table clientes(
id_cliente int primary key identity,
nombre varchar(60),
apellido varchar(60),
correo varchar(100),
contrasenia varchar(100),
codigo_postal int foreign key references ciudades(codigo_postal),
telefono varchar(60),
reestablecer bit default 0,
fecha_registro datetime default getdate())

go 

create table carrito_temporal(
id_carrito int primary key identity,
cantidad int,
id_cliente int foreign key references clientes(id_cliente),
id_producto int foreign key references productos(id_producto))

go

create table ventas(
id_venta int primary key identity,
id_cliente int foreign key references clientes(id_cliente),
precio_total decimal(18,2),
id_transaccion varchar(60),
direccion varchar(100),
fecha_registro datetime default getdate())

go

select * from ventas

create table detalle_ventas(
id_detalle_venta int primary key identity,
cantidad int,
precio_producto int,
precio_total decimal(18,2),
id_venta int foreign key references ventas(id_venta),
id_producto int foreign key references productos(id_producto)
)

create table usuarios(
id_usuario int primary key identity,
nombre varchar(60),
apellido varchar(60),
correo varchar(100),
contrasenia varchar (20),
reestablecer bit default 1,
activo bit default 1,
fecha_registro datetime default getdate())

select * from usuarios

--agregar el procedimiento almacenado de registrar, editar y eliminar usuario

--------------------------------------------------------------------------

/* PROCEDIMIENTOS ALMACENADOS*/

create proc sp_RegistrarCategoria(
    @Descripcion varchar(100),
    @Activo bit,
    @Mensaje varchar(500) output,
    @Resultado int output
)
as
begin
    -- Inicia la variable @Resultado a 0
    SET @Resultado = 0

    -- Verifica si ya existe una categoría con la misma descripción
    IF NOT EXISTS (SELECT * FROM categorias where nombre_categoria = @Descripcion)
    begin
        -- Si no existe, inserta la nueva categoría en la tabla "categorias"
        insert into categorías(nombre_categoria, activo) values (@Descripcion, @Activo)

        -- Obtiene el ID de la categoría recién registrada y la almacena en @Resultado
        SET @Resultado = SCOPE_IDENTITY()
    end
    else
    begin
        -- Si la categoría ya existe:
        set @Mensaje = 'La categoría ya existe'
    end
end

--------------------------------------------------------------------------

create proc sp_EditarCategoria(
    @IdCategoria int,
    @Descripcion varchar(100),
    @Activo bit,
    @Mensaje varchar(500) output,
    @Resultado bit output
)
as
begin
    -- Inicia la variable @Resultado a 0
    SET @Resultado = 0

    -- Verifica si ya existe una categoría con la misma descripción, excluyendo la categoría actual
    IF NOT EXISTS (SELECT * FROM categorias where nombre_categoria = @Descripcion and id_categoria != @IdCategoria)
    begin
        -- Si no existe una categoría con la misma descripción, realiza la edición
        update top(1) categorias set
        nombre_categoria = @Descripcion,
        activo = @Activo
        where id_categoria = @IdCategoria

        -- Establece @Resultado en 1 para indicar que la edición se hizo bien
        SET @Resultado = 1
    end
    else 
    begin
        -- Si ya existe una categoría con la misma descripción muestra:
        set @Mensaje = 'La categoría ya existe'
    end
end

--------------------------------------------------------------------------

create proc sp_EliminarCategoria(
@IdCategoria int,
@Mensaje varchar(500) output,
@Resultado bit output
)
as
begin 
	SET @Resultado = 0 -- verifica que no exista ningun producto asociado a la categoria
	IF NOT EXISTS (select * from productos p inner join categorias c on c.id_categoria = p.id_categoria where p.id_categoria = @IdCategoria)
	begin
		delete top (1) from categorias where id_categoria = @IdCategoria
		SET @Resultado = 1
		end
		else
		set @Mensaje = 'La categoria se encuentra relacionada a un producto'
	end

--------------------------------------------------------------------------

create proc sp_RegistrarMarca(
@Descripcion varchar(100),
@Activo bit,
@Mensaje varchar(500) output,
@Resultado int output
)
as
begin
	set @Resultado = 0
	if not exists(select * from marca where nombre_marca = @Descripcion)
	begin
	insert into marca(nombre_marca, activo) values(@Descripcion, @Activo)

	set @Resultado = SCOPE_IDENTITY()
	end
	else
	set @Mensaje = 'La marca ya existe'
end

--------------------------------------------------------------------------

create proc sp_EditarMarca(
@IdMarca int,
@Descripcion varchar(100),
@Activo bit,
@Mensaje varchar(500) output,
@Resultado bit output
)
as
begin
	set @Resultado = 0
	if not exists(select * from marca where nombre_marca = @Descripcion and id_marca != @IdMarca)
	begin

		update top (1) marca set nombre_marca = @Descripcion, activo = @Activo where id_marca = @IdMarca

		set @Resultado = 1
	end
	else 
	set @Mensaje = 'La marca ya existe'
end

--------------------------------------------------------------------------

create proc sp_EliminarMarca(
@IdMarca int,
@Mensaje varchar(500) output,
@Resultado bit output
)
as
begin
	set @Resultado = 0
	if not exists(select * from productos p inner join marca m on m.id_marca = p.id_marca where p.id_marca = @IdMarca)
	begin
		delete top (1) from marca where id_marca = @IdMarca
		set @Resultado = 1
	end
	else
	set @Mensaje = 'La marca se encuentra relacionada a un producto'
end


select id_marca, nombre_marca, activo from marca

insert into marca(nombre_marca) values('SONY')

--------------------------------------------------------------------------

create proc sp_RegistrarProducto(
@Nombre varchar(100),
@Descripcion varchar(100),
@IdMarca varchar(100),
@IdCategoria varchar(100),
@Precio decimal(10,2),
@Stock int,
@Activo bit,
@Mensaje varchar(500) output,
@Resultado int output
)
as
begin
	set @Resultado = 0
	if not exists(select * from productos where nombre = @Nombre)
	begin
		insert into productos(nombre, descripcion, id_marca, id_categoria, precio, stock, activo) values(@Nombre, @Descripcion, @IdMarca, @IdCategoria, @Precio, @Stock, @Activo)
		set @Resultado = SCOPE_IDENTITY()
	end
	else
	set @Mensaje = 'El producto ya existe'
end

--------------------------------------------------------------------------

create proc sp_EditarProducto(
@IdProducto int,
@Nombre varchar(100),
@Descripcion varchar(100),
@IdMarca varchar(100),
@IdCategoria varchar(100),
@Precio decimal(10,2),
@Stock int,
@Activo bit,
@Mensaje varchar(500) output,
@Resultado bit output
)
as
begin
	set @Resultado = 0
	if not exists(select * from productos where nombre = @Nombre and id_producto != @IdProducto)
	begin

		update productos set
		nombre = @Nombre, descripcion = @Descripcion, id_marca = @IdMarca, id_categoria = @IdCategoria, precio = @Precio, stock = @Stock, activo = @Activo where id_producto = @IdProducto

		set @Resultado = 1
	end
	else
	 set @Mensaje = 'El producto ya existe'
end

--------------------------------------------------------------------------

create proc sp_EliminarProducto(
@IdProducto int,
@Mensaje varchar(500) output,
@Resultado bit output
)
as
begin
	set @Resultado = 0
	if not exists(select * from detalle_ventas dv inner join productos p on p.id_producto = dv.id_producto where p.id_producto = @IdProducto)
	begin
		delete top (1) from productos where id_producto = @IdProducto
			set @Resultado = 1
	end
	else
	 set @Mensaje = 'El producto se encuentra relacionado a una venta'
end

--------------------------------------------------------------------------

create proc sp_ReporteDashboard
as
begin

select 

(select count(*) from clientes) [TotalCliente],
(select isnull(sum(cantidad),0) from detalle_ventas) [TotalVenta],
(select count(*) from productos) [TotalProducto]

end

exec sp_ReporteDashboard

--------------------------------------------------------------------------

create proc sp_ReporteVentas(
@fechainicio varchar(10),
@fechafin varchar(10),
@idtransaccion varchar(50)
)
as
begin 

set dateformat dmy;

select CONVERT(char(10),v.fecha_registro,103)[FechaVenta], CONCAT(c.nombre,' ',c.apellido)[Cliente], p.nombre[Producto], p.precio[Precio], dv.cantidad[Cantidad], dv.precio_total[Total], v.id_transaccion[IdTransaccion] from detalle_ventas dv 
inner join productos p on p.id_producto = dv.id_producto
inner join ventas v on v.id_venta = dv.id_venta
inner join clientes c on c.id_cliente = v.id_cliente
where CONVERT(date, v.fecha_registro) between @fechainicio and @fechafin
and v.id_transaccion = iif(@idtransaccion = '', v.id_transaccion, @idtransaccion) -- si no se ingresa un id_transaccion que se muestre el id_transaccion de la columna seleccionada. Sino que se muestre el id_Transaccion pegado
end

exec sp_ReporteVentas

--------------------------------------------------------------------------

create proc sp_RegistrarCliente(
@Nombres varchar(100),
@Apellidos varchar(100),
@Correo varchar(100),
@Clave varchar(100),
@Mensaje varchar(500) output,
@Resultado int output
)
as
begin
	set @Resultado = 0
	if not exists(select * from clientes where correo = @Correo)
	begin
		insert into clientes(nombre, apellido, correo, contrasenia, reestablecer) values(@Nombres, @Apellidos, @Correo, @Clave, 0)

		set @Resultado = SCOPE_IDENTITY()
	end
	 else
	  set @Mensaje = 'El correo del usuario ya existe'
end

--------------------------------------------------------------------------

create proc sp_RegistrarCliente(
@Nombres varchar(100),
@Apellidos varchar(100),
@Correo varchar(100),
@Clave varchar(100),
@CodigoPostal int, -- Usar el código postal de la ciudad
@Telefono varchar(60),
@Mensaje varchar(500) output,
@Resultado int output
)
as
begin
    set @Resultado = 0
    if not exists(select * from clientes where correo = @Correo)
    begin
        insert into clientes(nombre, apellido, correo, contrasenia, codigo_postal, telefono, reestablecer)
        values(@Nombres, @Apellidos, @Correo, @Clave, @CodigoPostal, @Telefono, 0)

        set @Resultado = SCOPE_IDENTITY()
    end
    else
        set @Mensaje = 'El correo del cliente ya existe'
end

-------------------------------------------------------------- PROCEDIMIENTO PARA QUE NO SE REPITAN LOS PRODUCTOS EN EL CARRITO

create proc sp_ExisteCarrito(
@IdCliente int,
@IdProducto int,
@Resultado bit output
)
as
begin
	if exists(select * from carrito_temporal where id_cliente = @IdCliente and id_producto = @IdProducto)
		set @Resultado = 1
	else 
		set @Resultado = 0 
end


---------------------------------------------------------------

create proc sp_OperacionCarrito(
    @IdCliente int,
    @IdProducto int,
    @Sumar bit,
    @Mensaje varchar(500) output,
    @Resultado bit output
)
as
begin
    -- Inicia @Resultado a 1 (bien)
    set @Resultado = 1
    set @Mensaje = ''

    -- Verifica si el producto ya existe en el carrito del cliente
    declare @existecarrito bit = iif(exists(select * from carrito_temporal where id_cliente = @IdCliente and id_producto = @IdProducto), 1, 0)

    -- Obtiene el stock de los productos
    declare @stockproducto int = (select stock from productos where id_producto = @IdProducto)

    begin try
        -- Inicia una transacción
        begin transaction OPERACION

        if (@Sumar = 1)
        begin
            -- Si la operación es sumar

            if (@stockproducto > 0)
            begin
                -- Verifica si hay stock disponible del producto

                if (@existecarrito = 1)
                    -- Si el producto ya está en el carrito, incrementa la cantidad en 1
                    update carrito_temporal set cantidad = cantidad + 1 where id_cliente = @IdCliente and id_producto = @IdProducto
                else
                    -- Si el producto no está en el carrito, lo agrega al carrito con cantidad 1
                    insert into carrito_temporal(id_cliente, id_producto, cantidad) values(@IdCliente, @IdProducto, 1)

                -- Reduce el stock del producto en 1
                update productos set stock = stock - 1 where id_producto = @IdProducto
            end
            else
            begin
                -- Si no hay stock disponible, establece @Resultado en 0 y @Mensaje en un mensaje de error
                set @Resultado = 0
                set @Mensaje = 'El producto no cuenta con stock disponible'
            end
        end
        else
        begin
            -- Si la operación es restar del carrito
            update carrito_temporal set cantidad = cantidad - 1 where id_cliente = @IdCliente and id_producto = @IdProducto
            -- Aumenta el stock del producto en 1
            update productos set stock = stock + 1 where id_producto = @IdProducto
        end

        -- Confirma la transacción 
        commit transaction OPERACION
    end try
    begin catch
        -- En caso de error, establece @Resultado en 0 y @Mensaje con el error 
        set @Resultado = 0
        set @Mensaje = ERROR_MESSAGE()

        ROLLBACK TRANSACTION OPERACION
    end catch
end


/* la variable existecarrito se esta validando si hay relacion entre el id_cliente y el id_producto

para la variable stockproducto se esta obteniendo el stock actual del producto que se esta solicitando

si hay algun error en la transaccion con el rollback se vuelve todo para atras */

--------------------------------------------------------------------------

create proc sp_EliminarCarrito(
@IdCliente int,
@IdProducto int,
@Resultado bit output
)
as
begin
	set @Resultado = 1
	declare @cantidadproducto int = (select cantidad from carrito_temporal where id_cliente = @IdCliente and id_producto = @IdProducto)

	begin try

		begin transaction OPERACION

		update productos set stock = stock + @cantidadproducto where id_producto = @IdProducto
		delete top (1) from carrito_temporal where id_cliente = @IdCliente and id_producto = @IdProducto

		commit transaction OPERACION
	 end try
	 begin catch
		set @Resultado = 0
		rollback transaction OPERACION
	end catch
end

--------------------------------------------------------------------------
--funcion

create function fn_obtenerCarritoCliente(
@idcliente int)
returns table 
as
return(select p.id_producto, m.nombre_marca[DesMarca], p.nombre, p.precio, c.cantidad, p.rutaImagen, p.nombreImagen from carrito_temporal c
inner join productos p on p.id_producto = c.id_producto
inner join marca m on m.id_marca = p.id_marca
where c.id_cliente = @idcliente)

select * from fn_obtenerCarritoCliente(20)
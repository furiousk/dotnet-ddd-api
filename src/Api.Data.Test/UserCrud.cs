using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using src.Api.Data.Context;
using src.Api.Data.Implementation;
using src.Api.Domain.Entities;
using Xunit;

namespace src.Api.Data.Test
{
    public class UserCrud : BaseTest, IClassFixture<DbTest>
    {
        private ServiceProvider _serviceProvider;

        public UserCrud(DbTest dbTest)
        {
            _serviceProvider = dbTest.ServiceProvider;
        }
        [Fact(DisplayName = "Crud de Usuario")]
        [Trait("Crud", "UserEntity")]
        public async Task Testando_Crud_de_Usuarios()
        {

            using (var context = _serviceProvider.GetService<ApplicationDbContext>())
            {
                var _repositorio = new UserImplementation(context);
                var _entity = new UserEntity
                {
                    Email = Faker.Internet.Email(),
                    Name = Faker.Name.FullName()
                };
                var _registroCriado = await _repositorio.InsertAsync(_entity);

                Assert.NotNull(_registroCriado);
                Assert.Equal(_entity.Email, _registroCriado.Email);
                Assert.Equal(_entity.Name, _registroCriado.Name);
                Assert.False(_registroCriado.Id == Guid.Empty);

                _entity.Name = Faker.Name.First();

                var _registroAtualizado = await _repositorio.UpdateAsync(_entity);

                Assert.NotNull(_registroAtualizado);
                Assert.Equal(_entity.Email, _registroAtualizado.Email);
                Assert.Equal(_entity.Name, _registroAtualizado.Name);

                var _registroExiste = await _repositorio.ExistsAsync(_registroAtualizado.Id);

                Assert.True(_registroExiste);

                var _registroSelecionado = await _repositorio.SelectAsync(_registroAtualizado.Id);

                Assert.NotNull(_registroSelecionado);
                Assert.Equal(_registroAtualizado.Email, _registroSelecionado.Email);
                Assert.Equal(_registroAtualizado.Name, _registroSelecionado.Name);

                var _todosRegistros = await _repositorio.SelectAsync();

                Assert.NotNull(_todosRegistros);
                Assert.True(_todosRegistros.Count() > 0);

                var _remove = await _repositorio.DeleteAsync(_registroSelecionado.Id);
                Assert.True(_remove);

                var _userDefault = await _repositorio.FindByLogin("furious@gmail.com");

                Assert.NotNull(_userDefault);
                Assert.Equal("furious@gmail.com", _userDefault.Email);
                Assert.Equal("Administrador", _userDefault.Name);
            }
        }
    }
}

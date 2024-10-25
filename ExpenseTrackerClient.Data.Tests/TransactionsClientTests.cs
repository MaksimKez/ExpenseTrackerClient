using System.Net;
using System.Net.Http.Json;
using ExpenseTrackerClient.Data.HttpClients;
using ExpenseTrackerClient.Data.Models;
using Moq;
using Moq.Protected;

namespace ExpenseTrackerClient.Data.Tests;

[TestFixture]
    public class TransactionsClientTests
    {
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private TransactionsClient _transactionsClient;

        [SetUp]
        public void Setup()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _transactionsClient = new TransactionsClient();
        }

        [Test]
        public async Task GetIncomesByBankAccountIdAsync_ShouldReturnIncomesList_WhenRequestIsSuccessful()
        {
            var incomes = new List<Income> { new Income { Id = Guid.NewGuid(), Sum = 1000 } };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(incomes)
                });

            var result = await _transactionsClient.GetIncomesByBankAccountIdAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(incomes[0].Sum, result[0].Sum);
        }

        [Test]
        public async Task GetIncomesByBankAccountIdAsync_ShouldReturnEmptyList_WhenRequestFails()
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var result = await _transactionsClient.GetIncomesByBankAccountIdAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task DeleteIncomeAsync_ShouldReturnTrue_WhenDeletionIsSuccessful()
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var result = await _transactionsClient.DeleteIncomeAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteIncomeAsync_ShouldReturnFalse_WhenDeletionFails()
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var result = await _transactionsClient.DeleteIncomeAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddIncomeAsync_ShouldReturnIncomeId_WhenRequestIsSuccessful()
        {
            var incomeId = Guid.NewGuid();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = JsonContent.Create(incomeId)
                });

            var result = await _transactionsClient.AddIncomeAsync(Guid.NewGuid(), new Income { Sum = 1000 });

            Assert.That(result, Is.EqualTo(incomeId));
        }

        [Test]
        public async Task AddIncomeAsync_ShouldReturnEmptyGuid_WhenRequestFails()
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var result = await _transactionsClient.AddIncomeAsync(Guid.NewGuid(), new Income { Sum = 1000 });

            Assert.That(result, Is.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task GetBankAccountByIdAsync_ShouldReturnBankAccount_WhenRequestIsSuccessful()
        {
            var bankAccount = new BankAccount { Id = Guid.NewGuid(), Balance = 500 };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(bankAccount)
                });

            var result = await _transactionsClient.GetBankAccountByIdAsync(bankAccount.Id);

            Assert.IsNotNull(result);
            Assert.That(result.Balance, Is.EqualTo(bankAccount.Balance));
        }

        [Test]
        public async Task GetBankAccountByIdAsync_ShouldReturnNull_WhenRequestFails()
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var result = await _transactionsClient.GetBankAccountByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }
    }
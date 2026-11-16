using Core.Domain.Common;

namespace Core.Tests.Domain;

public class PaginationTests
{
    [Fact]
    public void Constructor_PageBeyondLast_ClampsToLastPage()
    {
        var pagination = new Pagination(itemsCount: 25, pageSize: 10, page: 7);

        pagination.Page.ShouldBe(3);
        pagination.TotalPages.ShouldBe(3);
        pagination.HasNext.ShouldBeFalse();
        pagination.HasPrevious.ShouldBeTrue();
        pagination.Skip.ShouldBe(20);
        pagination.StartItem.ShouldBe(21);
        pagination.EndItem.ShouldBe(25);
    }

    [Fact]
    public void Constructor_PageSizeAboveLimit_UsesLimit()
    {
        var pagination = new Pagination(itemsCount: 1000, pageSize: 5000, page: 1);

        pagination.PageSize.ShouldBe(PaginationSettings.Default.MaxPageSize);
    }

    [Fact]
    public void Constructor_NoItems_ReturnsSingleEmptyPage()
    {
        var pagination = new Pagination(itemsCount: 0, pageSize: null, page: null);

        pagination.TotalPages.ShouldBe(1);
        pagination.Page.ShouldBe(1);
        pagination.StartItem.ShouldBe(0);
        pagination.EndItem.ShouldBe(0);
        pagination.HasNext.ShouldBeFalse();
    }
}

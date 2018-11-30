using FluentValidation;
using CloudFabric.SampleService.DTOs;

namespace CloudFabric.SampleService.Validators
{
    public class ProductValidator : AbstractValidator<ProductDTO>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Category).NotEmpty().WithMessage("Please specify a category");
            RuleFor(x => x.Subcategory).NotEmpty().WithMessage("Please specify a sub category");
        }
    }
}

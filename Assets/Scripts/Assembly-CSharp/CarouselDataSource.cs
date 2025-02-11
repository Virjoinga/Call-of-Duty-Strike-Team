using System.Collections.Generic;

public interface CarouselDataSource
{
	void Next();

	void Previous();

	void Populate(List<CarouselItem> items, int middleIndex);
}

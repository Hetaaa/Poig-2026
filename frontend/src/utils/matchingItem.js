import { getWeatherStatus } from "./weatherHelpers";

export function getMatchingOutfit(clothingItems, weather){
    const weatherStatus = getWeatherStatus(weather)
    const temperature = weather?.temperature;

    if(!clothingItems || !weather) return [];

    return clothingItems.filter((item) => {
        if (weatherStatus ==="raining" && !item.waterproof) return false;
        if (weatherStatus === "windy" && !item.windproof) return false;
        
        if (temperature < 5 && item.warmth<4) return false; 
        if (temperature >=5 && temperature < 15 && item.warmth <3) return false; 
        if (temperature >=15 && temperature < 23 && item.warmth <2) return false; 
        if (temperature >= 23 && item.warmth > 2) return false; 

        return true;
    });
}

export function getRandomItem(items){
    if (!items || items.length===0) return null; 

    const randomIndex = Math.floor(Math.random() * items.length);
    return items[randomIndex]
}

export function getRandomMatchingOutfit(clothingItems, weather) {
    const matchingItems = getMatchingOutfit(clothingItems, weather);

    const base = getRandomItem(
        matchingItems.filter((item)=> item.layer === "Warstwa bazowa")
    );

    const middle = getRandomItem(
        matchingItems.filter((item)=> item.layer === "Warstwa średnia")
    );

    const bottom = getRandomItem(
        matchingItems.filter((item)=> item.layer === "Dół")
    );

    const shoes = getRandomItem(
        matchingItems.filter((item)=> item.layer === "Obuwie")
    );

    const outfit = []
    if (base) outfit.push(base);
    if (middle) outfit.push(middle);
    if (bottom) outfit.push(bottom);
    if (shoes) outfit.push(shoes);

    return outfit;
}


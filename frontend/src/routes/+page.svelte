<script lang="ts">
	import type { ImageData } from '$lib/types';
	import Preloader from '$lib/components/preloader.svelte';

	let loading = false;
	let imageData: ImageData | null = null;

	async function generate(): Promise<void> {
		loading = true;
		imageData = null;
		const url = 'https://randomimagegenerator.azurewebsites.net/api/GenerateImageLink';
		const request = JSON.stringify({ textgen: 'OpenAI', imagegen: 'OpenAI' });
		const result = await fetch(url, { method: 'POST', body: request });
		const body = await result.json();
		imageData = body;
		loading = false;
	}
</script>

<div>
	<button on:click={generate} disabled={loading}>Generate</button>
</div>

{#if loading}
	<div>
		<Preloader />
	</div>
{/if}

{#if imageData !== null}
	<div>
		<figure>
			<img src={imageData.link} alt="AI-generated" />
			<figcaption>{imageData.sentence}</figcaption>
		</figure>
	</div>
{/if}
